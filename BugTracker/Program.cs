using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using BugTracker.Data;
using BugTracker.Middleware;
using BugTracker.Repositories;
using BugTracker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger com suporte a JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BugTracker API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization. Digite: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// CORS para o frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Injeção de dependências
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProjetoRepository, ProjetoRepository>();
builder.Services.AddScoped<IBugRepository, BugRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IProjetoService, ProjetoService>();
builder.Services.AddScoped<IBugService, BugService>();
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
builder.Services.AddScoped<IComentarioService, ComentarioService>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IBugHistoricoRepository, BugHistoricoRepository>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
var defaultFiles = new DefaultFilesOptions();
defaultFiles.DefaultFileNames.Clear();
defaultFiles.DefaultFileNames.Add("landing.html");
app.UseDefaultFiles(defaultFiles);
app.UseStaticFiles();
app.MapControllers();

// Inicializar banco de dados e seed do admin
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        db.Database.Migrate();
        logger.LogInformation("Migrations aplicadas com sucesso.");

        if (!db.Usuarios.Any(u => u.Perfil == "Admin"))
        {
            db.Usuarios.Add(new BugTracker.Models.Usuario
            {
                Nome = "Administrador",
                Email = "admin@bugtracker.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Perfil = "Admin",
                Cargo = "Lider",
                AceitouTermos = true,
                AceitouTermosEm = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
            logger.LogInformation("Admin padrão criado: admin@bugtracker.com / Admin@123");
        }

        // Seed de dados de demonstração
        if (!db.Projetos.Any())
        {
            var admin = db.Usuarios.First(u => u.Perfil == "Admin");

            var lucas = new BugTracker.Models.Usuario
            {
                Nome = "Lucas Silva",
                Email = "lucas@bugtracker.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Dev@123"),
                Perfil = "Dev",
                Cargo = "Programador",
                AceitouTermos = true,
                AceitouTermosEm = DateTime.UtcNow.AddDays(-30)
            };
            var ana = new BugTracker.Models.Usuario
            {
                Nome = "Ana Costa",
                Email = "ana@bugtracker.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Dev@123"),
                Perfil = "Dev",
                Cargo = "QA",
                AceitouTermos = true,
                AceitouTermosEm = DateTime.UtcNow.AddDays(-25)
            };
            db.Usuarios.AddRange(lucas, ana);
            await db.SaveChangesAsync();

            var hollow = new BugTracker.Models.Projeto
            {
                Nome = "Hollow Depths",
                Descricao = "Action RPG de masmorra com geração procedural. Ambientação dark fantasy com sistema de crafting e mundo aberto subterrâneo.",
                MotorJogo = "Unity",
                PlataformasAlvo = "PC, PlayStation 5",
                VersaoAtual = "0.8.2",
                CriadoEm = DateTime.UtcNow.AddDays(-60)
            };
            var starRaiders = new BugTracker.Models.Projeto
            {
                Nome = "Star Raiders Online",
                Descricao = "Shooter espacial multiplayer competitivo. Batalhas 8v8 em arenas com física de gravidade zero e sistemas de upgrades de nave.",
                MotorJogo = "Unreal Engine",
                PlataformasAlvo = "PC, Xbox Series X/S",
                VersaoAtual = "2.1.0",
                CriadoEm = DateTime.UtcNow.AddDays(-45)
            };
            var pixelFarm = new BugTracker.Models.Projeto
            {
                Nome = "Pixel Farm",
                Descricao = "Farming simulator com pixel art. Plante, colha, crie animais e construa sua fazenda dos sonhos em um mundo cheio de NPCs e missões.",
                MotorJogo = "Godot",
                PlataformasAlvo = "PC, Nintendo Switch, Android",
                VersaoAtual = "1.3.5",
                CriadoEm = DateTime.UtcNow.AddDays(-20)
            };
            db.Projetos.AddRange(hollow, starRaiders, pixelFarm);
            await db.SaveChangesAsync();

            var bugs = new List<BugTracker.Models.Bug>
            {
                // Hollow Depths
                new() {
                    Titulo = "Crash ao abrir baú de itens na Caverna das Sombras",
                    Descricao = "1. Entrar na Caverna das Sombras (Fase 4)\n2. Localizar o baú dourado próximo ao checkpoint\n3. Pressionar E para abrir\n4. Jogo fecha imediatamente sem mensagem de erro",
                    Severidade = "Critica", Status = "Aberto", TipoBug = "Crash",
                    Plataforma = "PC (Windows)", VersaoJogo = "0.8.2", NumeroBuild = "#1104",
                    Milestone = "v1.0 Launch", Cena = "Caverna das Sombras — Checkpoint 2",
                    FrequenciaReproducao = "Sempre (100%)", BloqueiaLancamento = true,
                    ResultadoEsperado = "Baú abre e exibe os itens na interface de inventário",
                    ResultadoObtido = "AccessViolationException — crash instantâneo ao acessar ItemDatabase",
                    DetalhesAmbiente = "GPU RTX 3060, 16GB RAM, Windows 11, Configuração Gráfica: Alta",
                    ProjetoId = hollow.Id, ReportadoPorId = admin.Id, AtribuidoParaId = lucas.Id,
                    CriadoEm = DateTime.UtcNow.AddDays(-15)
                },
                new() {
                    Titulo = "Textura do boss 'Sombra Anciã' não carrega no PS5",
                    Descricao = "1. Iniciar o jogo no PS5\n2. Progredir até o Boss da Fase 6\n3. Entrar na arena do boss\n4. Observar modelo do boss",
                    Severidade = "Alta", Status = "EmAndamento", TipoBug = "Visual",
                    Plataforma = "PlayStation 5", VersaoJogo = "0.8.2", NumeroBuild = "#1104",
                    Milestone = "v1.0 Launch", Cena = "Arena do Boss — Fase 6",
                    FrequenciaReproducao = "Sempre (100%)", BloqueiaLancamento = false,
                    ResultadoEsperado = "Boss exibe textura 4K com efeitos de partícula corretos",
                    ResultadoObtido = "Boss renderiza todo na cor magenta (textura fallback), sem partículas",
                    DetalhesAmbiente = "PlayStation 5, modo Performance (60fps), versão do sistema: 24.01",
                    ProjetoId = hollow.Id, ReportadoPorId = lucas.Id, AtribuidoParaId = ana.Id,
                    CriadoEm = DateTime.UtcNow.AddDays(-12)
                },
                new() {
                    Titulo = "FPS cai para menos de 20 na sala do Boss Final",
                    Descricao = "1. Chegar à sala do Boss Final (Fase 8)\n2. Iniciar a batalha\n3. Aguardar o boss usar o ataque de área 'Tempestade Sombria'\n4. Observar o framerate cair drasticamente",
                    Severidade = "Alta", Status = "EmAndamento", TipoBug = "Performance",
                    Plataforma = "PC (Windows)", VersaoJogo = "0.8.2", NumeroBuild = "#1104",
                    Milestone = "v1.0 Launch", Cena = "Sala do Boss Final — Fase 8",
                    FrequenciaReproducao = "Sempre (100%)", BloqueiaLancamento = true,
                    ResultadoEsperado = "Jogo mantém 60fps estáveis durante todo o combate",
                    ResultadoObtido = "FPS cai de 60 para 12-18 durante o ataque de área, recupera após 8 segundos",
                    DetalhesAmbiente = "GPU GTX 1080, 8GB RAM, Windows 10, todos os efeitos ativados",
                    ProjetoId = hollow.Id, ReportadoPorId = ana.Id, AtribuidoParaId = ana.Id,
                    CriadoEm = DateTime.UtcNow.AddDays(-10)
                },
                new() {
                    Titulo = "Personagem fica preso entre rochas na entrada da Fase 4",
                    Descricao = "1. Correr em direção à entrada da Fase 4\n2. Pressionar Sprint + Dodge simultaneamente ao passar pela fresta entre as rochas\n3. Personagem fica preso sem conseguir se mover",
                    Severidade = "Media", Status = "Resolvido", TipoBug = "Gameplay",
                    Plataforma = "PC (Windows)", VersaoJogo = "0.8.1", NumeroBuild = "#1098",
                    Cena = "Entrada da Fase 4 — Rochas do Portal",
                    FrequenciaReproducao = "Às Vezes (~50%)", BloqueiaLancamento = false,
                    ResultadoEsperado = "Personagem passa normalmente ou é bloqueado pelo colisor",
                    ResultadoObtido = "Personagem fica encravado, única saída é recarregar o save",
                    ProjetoId = hollow.Id, ReportadoPorId = lucas.Id, AtribuidoParaId = lucas.Id,
                    CriadoEm = DateTime.UtcNow.AddDays(-20), AtualizadoEm = DateTime.UtcNow.AddDays(-8)
                },
                new() {
                    Titulo = "Texto de habilidade passiva truncado no idioma PT-BR",
                    Descricao = "1. Mudar idioma para Português Brasileiro\n2. Abrir menu de habilidades\n3. Verificar a descrição da habilidade 'Escudo das Trevas'",
                    Severidade = "Baixa", Status = "Fechado", TipoBug = "Localizacao",
                    Plataforma = "PC (Windows)", VersaoJogo = "0.8.0",
                    FrequenciaReproducao = "Sempre (100%)", BloqueiaLancamento = false,
                    ResultadoEsperado = "Texto completo exibido dentro da caixa de descrição",
                    ResultadoObtido = "Texto cortado na terceira linha, falta parte da descrição do efeito",
                    ProjetoId = hollow.Id, ReportadoPorId = ana.Id,
                    CriadoEm = DateTime.UtcNow.AddDays(-25), AtualizadoEm = DateTime.UtcNow.AddDays(-18)
                },

                // Star Raiders Online
                new() {
                    Titulo = "Crash ao entrar em lobby com 8 jogadores simultâneos",
                    Descricao = "1. Criar um lobby privado\n2. Convidar 7 jogadores adicionais\n3. Assim que o 8º jogador entra no lobby\n4. Servidor desconecta todos os jogadores com crash",
                    Severidade = "Critica", Status = "Aberto", TipoBug = "Crash",
                    Plataforma = "PC (Windows)", VersaoJogo = "2.1.0", NumeroBuild = "#2205",
                    Milestone = "Patch 2.2", Cena = "Lobby Multiplayer",
                    FrequenciaReproducao = "Frequentemente (>50%)", BloqueiaLancamento = true,
                    ResultadoEsperado = "Todos os 8 jogadores entram no lobby e podem iniciar a partida",
                    ResultadoObtido = "NullReferenceException no servidor ao sincronizar estado do 8º jogador",
                    DetalhesAmbiente = "Servidor dedicado, 10 Gbps, Windows Server 2022",
                    ProjetoId = starRaiders.Id, ReportadoPorId = admin.Id, AtribuidoParaId = lucas.Id,
                    CriadoEm = DateTime.UtcNow.AddDays(-8)
                },
                new() {
                    Titulo = "Colisão de nave permanece ativa após destruição",
                    Descricao = "1. Destruir uma nave inimiga\n2. Tentar voar pelo local onde a nave estava\n3. A colisão invisível ainda bloqueia o movimento",
                    Severidade = "Alta", Status = "EmAndamento", TipoBug = "Gameplay",
                    Plataforma = "PC (Windows)", VersaoJogo = "2.1.0", NumeroBuild = "#2205",
                    Cena = "Arena Nebulosa Vermelha — Combate",
                    FrequenciaReproducao = "Sempre (100%)", BloqueiaLancamento = false,
                    ResultadoEsperado = "Colisão da nave é removida junto com o modelo ao ser destruída",
                    ResultadoObtido = "Hitbox permanece no espaço por 15-20 segundos após explosão",
                    ProjetoId = starRaiders.Id, ReportadoPorId = lucas.Id, AtribuidoParaId = ana.Id,
                    CriadoEm = DateTime.UtcNow.AddDays(-6)
                },
                new() {
                    Titulo = "Tela de ranking não atualiza em tempo real durante a partida",
                    Descricao = "1. Iniciar uma partida multiplayer\n2. Abrir a tela de ranking (Tab)\n3. Observar que os valores de kills e pontos não mudam",
                    Severidade = "Media", Status = "Aberto", TipoBug = "UI",
                    Plataforma = "PC (Windows)", VersaoJogo = "2.1.0",
                    FrequenciaReproducao = "Sempre (100%)", BloqueiaLancamento = false,
                    ResultadoEsperado = "Ranking atualiza a cada kill, refletindo placar em tempo real",
                    ResultadoObtido = "Ranking só atualiza ao fechar e reabrir a tela (F5)",
                    ProjetoId = starRaiders.Id, ReportadoPorId = ana.Id,
                    CriadoEm = DateTime.UtcNow.AddDays(-5)
                },
                new() {
                    Titulo = "Shader das estrelas pisca em monitores ultrawide (21:9)",
                    Descricao = "1. Configurar resolução para 3440x1440 (21:9)\n2. Entrar em qualquer mapa com fundo estelar\n3. Mover a câmera rapidamente",
                    Severidade = "Baixa", Status = "Resolvido", TipoBug = "Visual",
                    Plataforma = "PC (Windows)", VersaoJogo = "2.0.9",
                    FrequenciaReproducao = "Sempre (100%)", BloqueiaLancamento = false,
                    ResultadoEsperado = "Campo estelar renderiza suavemente em qualquer proporção",
                    ResultadoObtido = "Artefatos de flickering nas bordas laterais em aspect ratio 21:9",
                    DetalhesAmbiente = "Monitor LG 34\" 3440x1440, GPU RX 6700 XT, Driver AMD 23.12",
                    ProjetoId = starRaiders.Id, ReportadoPorId = lucas.Id,
                    CriadoEm = DateTime.UtcNow.AddDays(-18), AtualizadoEm = DateTime.UtcNow.AddDays(-11)
                },

                // Pixel Farm
                new() {
                    Titulo = "Culturas não crescem após evento de chuva em tiles específicos",
                    Descricao = "1. Plantar cenouras nos tiles da área nordeste do mapa\n2. Aguardar o evento de chuva (dia 3, 18h in-game)\n3. Avançar 2 dias no jogo\n4. Verificar crescimento das plantas",
                    Severidade = "Alta", Status = "EmAndamento", TipoBug = "Gameplay",
                    Plataforma = "PC (Windows)", VersaoJogo = "1.3.5", NumeroBuild = "#351",
                    Milestone = "v1.4 Content Update", Cena = "Fazenda — Área Nordeste",
                    FrequenciaReproducao = "Frequentemente (>50%)", BloqueiaLancamento = true,
                    ResultadoEsperado = "Plantas avançam 1 estágio de crescimento após chuva",
                    ResultadoObtido = "Plantas ficam congeladas no estágio 1, TileWaterSystem não aplica umidade nos tiles afetados",
                    ProjetoId = pixelFarm.Id, ReportadoPorId = admin.Id, AtribuidoParaId = lucas.Id,
                    CriadoEm = DateTime.UtcNow.AddDays(-4)
                },
                new() {
                    Titulo = "Personagem atravessa cerca ao correr em diagonal",
                    Descricao = "1. Aproximar-se da cerca do galinheiro em diagonal\n2. Pressionar Sprint + direção diagonal (↑→)\n3. O personagem atravessa a cerca",
                    Severidade = "Media", Status = "Aberto", TipoBug = "Gameplay",
                    Plataforma = "Nintendo Switch", VersaoJogo = "1.3.5",
                    Cena = "Galinheiro — Cerca Lateral",
                    FrequenciaReproducao = "Às Vezes (~50%)", BloqueiaLancamento = false,
                    ResultadoEsperado = "Personagem é bloqueado pelo colisor da cerca",
                    ResultadoObtido = "Personagem atravessa a cerca e fica dentro do galinheiro sem conseguir sair",
                    ProjetoId = pixelFarm.Id, ReportadoPorId = ana.Id,
                    CriadoEm = DateTime.UtcNow.AddDays(-3)
                },
                new() {
                    Titulo = "Crash ao tentar plantar com inventário completamente cheio",
                    Descricao = "1. Encher o inventário com 40/40 itens\n2. Equipar uma semente na hotbar\n3. Selecionar um tile de terra\n4. Clicar para plantar",
                    Severidade = "Critica", Status = "Aberto", TipoBug = "Crash",
                    Plataforma = "Android", VersaoJogo = "1.3.5", NumeroBuild = "#351",
                    Milestone = "Hotfix 1.3.6", Cena = "Qualquer área plantável",
                    FrequenciaReproducao = "Sempre (100%)", BloqueiaLancamento = true,
                    ResultadoEsperado = "Exibir mensagem 'Inventário cheio' e não plantar",
                    ResultadoObtido = "IndexOutOfRangeException — app fecha imediatamente",
                    DetalhesAmbiente = "Samsung Galaxy A54, Android 14, 6GB RAM",
                    ProjetoId = pixelFarm.Id, ReportadoPorId = lucas.Id, AtribuidoParaId = ana.Id,
                    CriadoEm = DateTime.UtcNow.AddDays(-2)
                },
                new() {
                    Titulo = "Loja da Vila fecha às 17h mas deveria fechar às 18h",
                    Descricao = "1. Verificar descrição da loja: 'Aberta das 8h às 18h'\n2. Tentar acessar a loja às 17h05 (horário in-game)\n3. Loja já está fechada",
                    Severidade = "Baixa", Status = "Resolvido", TipoBug = "Gameplay",
                    Plataforma = "PC (Windows)", VersaoJogo = "1.3.4",
                    Cena = "Vila Central — Loja do Mercador",
                    FrequenciaReproducao = "Sempre (100%)", BloqueiaLancamento = false,
                    ResultadoEsperado = "Loja acessível até às 18h conforme descrito",
                    ResultadoObtido = "Variável ShopCloseHour estava hardcoded como 17 ao invés de 18",
                    ProjetoId = pixelFarm.Id, ReportadoPorId = ana.Id,
                    CriadoEm = DateTime.UtcNow.AddDays(-14), AtualizadoEm = DateTime.UtcNow.AddDays(-13)
                },
            };

            db.Bugs.AddRange(bugs);
            await db.SaveChangesAsync();

            var comentarios = new List<BugTracker.Models.Comentario>
            {
                new() { BugId = bugs[0].Id, UsuarioId = lucas.Id, Texto = "Consigo reproduzir consistentemente. O crash acontece no método ItemDatabase.GetItemById() — parece um ID nulo sendo passado. Vou investigar o gerador de loot do baú.", CriadoEm = DateTime.UtcNow.AddDays(-14) },
                new() { BugId = bugs[0].Id, UsuarioId = admin.Id, Texto = "Prioridade máxima, esse baú é obrigatório para completar a missão principal. Precisamos de um fix antes do build de QA na sexta.", CriadoEm = DateTime.UtcNow.AddDays(-13) },
                new() { BugId = bugs[0].Id, UsuarioId = lucas.Id, Texto = "Encontrei o problema — o ChestLootTable não inicializa o ItemDatabase quando o jogador está offline. Fix em desenvolvimento.", CriadoEm = DateTime.UtcNow.AddDays(-12) },

                new() { BugId = bugs[1].Id, UsuarioId = ana.Id, Texto = "Confirmado no PS5. O bundle de texturas do boss não está sendo incluído no build de console — só acontece no PS5, PC está OK.", CriadoEm = DateTime.UtcNow.AddDays(-11) },
                new() { BugId = bugs[1].Id, UsuarioId = admin.Id, Texto = "Verificar as configurações de addressables no build pipeline do PS5. Pode ser uma textura não marcada como 'include in build' para a plataforma.", CriadoEm = DateTime.UtcNow.AddDays(-10) },

                new() { BugId = bugs[5].Id, UsuarioId = lucas.Id, Texto = "Reproduzido no ambiente de testes com 8 bots. O crash ocorre no GameSessionManager.SyncPlayerState() quando o índice chega a 7 (0-based). Array de estados está com tamanho 7 ao invés de 8.", CriadoEm = DateTime.UtcNow.AddDays(-7) },
                new() { BugId = bugs[5].Id, UsuarioId = admin.Id, Texto = "Parece um off-by-one clássico. Confirme e faça o fix — esse bug está bloqueando os testes de QA multiplayer.", CriadoEm = DateTime.UtcNow.AddDays(-7) },

                new() { BugId = bugs[9].Id, UsuarioId = lucas.Id, Texto = "Fix aplicado no build #356. O TileWaterSystem não estava iterando pelos tiles usando coordenadas normalizadas — causava mapeamento errado na área nordeste.", CriadoEm = DateTime.UtcNow.AddDays(-3) },
                new() { BugId = bugs[9].Id, UsuarioId = admin.Id, Texto = "Ótimo. Vou marcar como resolvido após confirmação no build de QA hoje à tarde.", CriadoEm = DateTime.UtcNow.AddDays(-3) },

                new() { BugId = bugs[11].Id, UsuarioId = ana.Id, Texto = "Reproduzido no Android físico. O InventoryManager.PlantSeed() não valida se há espaço antes de tentar remover o item. Vou fazer a validação de borda.", CriadoEm = DateTime.UtcNow.AddDays(-1) },
            };

            db.Comentarios.AddRange(comentarios);
            await db.SaveChangesAsync();

            if (!db.Tags.Any())
            {
                db.Tags.AddRange(
                    new BugTracker.Models.Tag { Nome = "Arte/Animação", Cor = "#a855f7", Departamento = "Arte" },
                    new BugTracker.Models.Tag { Nome = "Áudio", Cor = "#f97316", Departamento = "Áudio" },
                    new BugTracker.Models.Tag { Nome = "Gameplay", Cor = "#22c55e", Departamento = "Gameplay" },
                    new BugTracker.Models.Tag { Nome = "UI/UX", Cor = "#06b6d4", Departamento = "UI/UX" },
                    new BugTracker.Models.Tag { Nome = "Performance", Cor = "#ef4444", Departamento = "Performance" },
                    new BugTracker.Models.Tag { Nome = "Rede/Online", Cor = "#3b82f6", Departamento = "Rede" },
                    new BugTracker.Models.Tag { Nome = "Localização", Cor = "#eab308", Departamento = "Localização" },
                    new BugTracker.Models.Tag { Nome = "Física", Cor = "#64748b", Departamento = "Gameplay" }
                );
                await db.SaveChangesAsync();
            }

            logger.LogInformation("Seed de demonstração criado: 3 projetos, {Bugs} bugs, {Comentarios} comentários.", bugs.Count, comentarios.Count);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao inicializar banco de dados: {Message}", ex.Message);
        throw;
    }
}

await app.RunAsync();
