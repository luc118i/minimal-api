using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enus;
using minimal_api.Dominio.ModelViews;
using minimal_api.Dominio.Serviços;
using minimal_api.Infraestutura.Db;
using minimal_api.Infraestutura.Interfaces;

namespace minimal_api.Api
{


    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
        }

        private string key = "";
        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(Options =>
            {
                Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(Options =>
            {
                Options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            services.AddAuthorization();


            services.AddScoped<iAdministradorServico, AdministradorServico>();
            services.AddScoped<iVeiculoServico, VeiculoServico>();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT aqui: "
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string []{}
                }
                });


            });


            services.AddDbContext<DbContexto>(options =>
            {
                options.UseSqlServer(
                    Configuration.GetConnectionString("sqlserver")
                );
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                #region Administradores
                string GerarTkenJwt(Administrador administrador)
                {
                    if (string.IsNullOrEmpty(key)) return string.Empty;

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var claims = new List<Claim>(){
                    new Claim("Email", administrador.Email),
                    new Claim("Perfil", administrador.Perfil),
                    new Claim(ClaimTypes.Role, administrador.Perfil),

                    };

                    var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: credentials
                    );
                    return new JwtSecurityTokenHandler().WriteToken(token);
                }

                endpoints.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, iAdministradorServico administradorServico) =>
                {
                    var adm = administradorServico.Login(loginDTO);
                    if (adm != null)
                    {
                        string token = GerarTkenJwt(adm);
                        return Results.Ok(new AdmLogado
                        {
                            Email = adm.Email,
                            Perfil = adm.Perfil,
                            Token = token
                        });
                    }
                    else
                    {
                        return Results.Unauthorized();
                    }
                }).AllowAnonymous().WithTags("Administrador");

                endpoints.MapGet("/administradores", ([FromQuery] int? pagina, iAdministradorServico administradorServico) =>
                {
                    var adms = new List<AdministradorModelViews>();
                    var administradores = administradorServico.Todos(pagina);
                    foreach (var adm in administradores)
                    {
                        adms.Add(new AdministradorModelViews
                        {
                            Id = adm.Id,
                            Email = adm.Email,
                            Perfil = adm.Perfil
                        });
                    }
                    return Results.Ok(adms);

                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Administrador");

                endpoints.MapGet("/Administradores/{id}", ([FromRoute] int id, iAdministradorServico administradorServico) =>
                {
                    var administrador = administradorServico.BuscaPorId(id);
                    if (administrador == null) return Results.NotFound();
                    return Results.Ok(new AdministradorModelViews
                    {
                        Id = administrador.Id,
                        Email = administrador.Email,
                        Perfil = administrador.Perfil
                    });

                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Administrador");

                endpoints.MapPut("/administradores/{id}", ([FromRoute] int id, [FromBody] Administrador administradorAtualizado,
                iAdministradorServico administradorServico) =>
                {
                    var administradorExistente = administradorServico.BuscaPorId(id);
                    if (administradorExistente == null)
                    {
                        return Results.NotFound();
                    }

                    administradorAtualizado.Id = id;
                    var administradorEditado = administradorServico.Editar(administradorAtualizado);

                    if (administradorEditado == null)
                    {
                        return Results.Problem("Erro ao editar o administrador.");
                    }
                    return Results.Ok(new
                    {
                        Id = administradorEditado.Id,
                        Email = administradorEditado.Email,
                        Perfil = administradorEditado.Perfil
                    });

                }).RequireAuthorization()
                    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                    .WithTags("Administrador");

                endpoints.MapDelete("/administradores/{id}", ([FromRoute] int id, iAdministradorServico administradorServico) =>
                {
                    var administrador = administradorServico.BuscaPorId(id);
                    if (administrador == null)
                    {
                        return Results.NotFound("Administrador não encontrado.");
                    }

                    var excluido = administradorServico.Excluir(id);
                    if (!excluido)
                    {
                        return Results.Problem("Erro ao excluir o administrador.");
                    }

                    return Results.Ok("Administrador excluído com sucesso.");
                }).RequireAuthorization()
                    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                    .WithTags("Administrador");




                endpoints.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, iAdministradorServico administradorServico) =>
                {
                    var validacao = new ErrosDeValidacao
                    {
                        Mensagens = new List<string>()
                    };

                    if (string.IsNullOrEmpty(administradorDTO.Email))
                        validacao.Mensagens.Add("Email não pode estar vazio!");

                    if (string.IsNullOrEmpty(administradorDTO.Senha))
                        validacao.Mensagens.Add("Senha não pode estar vazio!");

                    if (administradorDTO.Perfil == null)
                        validacao.Mensagens.Add("Perfil não pode estar vazio!");

                    if (validacao.Mensagens.Count > 0)
                        return Results.BadRequest(validacao);


                    var administrador = new Administrador
                    {
                        Email = administradorDTO.Email,
                        Senha = administradorDTO.Senha,
                        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()

                    };

                    administradorServico.Incluir(administrador);

                    return Results.Created($"/veiculo/{administrador.Id}", new AdministradorModelViews
                    {
                        Id = administrador.Id,
                        Email = administrador.Email,
                        Perfil = administrador.Perfil
                    });

                }).RequireAuthorization()
                    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                    .WithTags("Administrador");
                #endregion

                #region Veiculos
                ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
                {
                    var validacao = new ErrosDeValidacao
                    {
                        Mensagens = new List<string>()
                    };

                    if (string.IsNullOrEmpty(veiculoDTO.Nome))
                        validacao.Mensagens.Add("O nome não pode ser vazio");

                    if (string.IsNullOrEmpty(veiculoDTO.Marca))
                        validacao.Mensagens.Add("A marca não pode ser vazio");

                    if (veiculoDTO.Ano < 1945)
                        validacao.Mensagens.Add("O veiculo e muito antigo");

                    return validacao;



                }
                endpoints.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, iVeiculoServico veiculoServico) =>
                {

                    var validacao = validaDTO(veiculoDTO);
                    if (validacao.Mensagens.Count > 0)
                        return Results.BadRequest(validacao);


                    var veiculo = new Veiculo
                    {
                        Nome = veiculoDTO.Nome,
                        Marca = veiculoDTO.Marca,
                        Ano = veiculoDTO.Ano,
                    };
                    veiculoServico.Incluir(veiculo);

                    return Results.Created($"/veiculo/{veiculo}", veiculo);
                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
                .WithTags("Veiculos");
                //! Retornar todos os veiculos baseado na sua pagina
                endpoints.MapGet("/veiculos", ([FromQuery] int? pagina, iVeiculoServico veiculoServico) =>
                {


                    var veiculos = veiculoServico.Todos(pagina);

                    return Results.Ok(veiculos);
                }).RequireAuthorization()
                    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
                    .WithTags("Veiculos");

                //! Retornar o veiculo com base no seu id
                endpoints.MapGet("/veiculos/{id}", ([FromRoute] int id, iVeiculoServico veiculoServico) =>
                {
                    var veiculo = veiculoServico.BuscarPorId(id);

                    if (veiculo == null) return Results.NotFound();

                    return Results.Ok(veiculo);
                }).RequireAuthorization()
                    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
                    .WithTags("Veiculos");

                //! Alterar o veiculo
                endpoints.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, iVeiculoServico veiculoServico) =>
                {
                    var veiculo = veiculoServico.BuscarPorId(id);
                    if (veiculo == null) return Results.NotFound();

                    var validacao = validaDTO(veiculoDTO);
                    if (validacao.Mensagens.Count > 0)
                        return Results.BadRequest(validacao);



                    veiculo.Nome = veiculoDTO.Nome;
                    veiculo.Marca = veiculoDTO.Marca;
                    veiculo.Ano = veiculoDTO.Ano;

                    veiculoServico.Atualizar(veiculo);

                    return Results.Ok(veiculo);
                }).RequireAuthorization()
                    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                    .WithTags("Veiculos");

                //! Excluir o veiculo
                endpoints.MapDelete("/veiculos/{id}", ([FromRoute] int id, iVeiculoServico veiculoServico) =>
                {
                    var veiculo = veiculoServico.BuscarPorId(id);

                    if (veiculo == null) return Results.NotFound();

                    veiculoServico.Excluir(veiculo);

                    return Results.NoContent();
                }).RequireAuthorization()
                    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                    .WithTags("Veiculos");

                #endregion
            });
        }
    }
}