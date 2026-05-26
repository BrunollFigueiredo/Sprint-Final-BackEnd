# CLAUDE.md — Bug Tracker API

## Contexto do Projeto

Sistema de Bug Tracker voltado para equipes de desenvolvimento de jogos. O objetivo é registrar problemas encontrados, acompanhar o status de cada um e controlar quem é responsável por resolver.

Este projeto é uma atividade acadêmica que exige uma API REST completa em ASP.NET Core, integrada com banco de dados relacional, autenticação JWT, documentação Swagger e uma interface web responsiva consumindo a API.

---

## Domínio do Sistema

### Entidades Principais

- **Usuário** — quem usa o sistema, com perfis de acesso diferenciados
- **Projeto** — o jogo ou software sendo desenvolvido
- **Bug** — o problema reportado, vinculado a um projeto e a um usuário responsável
- **Comentário** — mensagem vinculada a um bug, postada por qualquer usuário autenticado

### Perfis de Acesso (Roles)

| Perfil | Permissões |
|--------|-----------|
| Admin | Acesso total: criar projetos, gerenciar usuários (promover/rebaixar/excluir), atribuir e excluir bugs |
| Dev | Reportar bugs, atualizar status dos bugs **atribuídos a ele**, postar comentários |

> Registro público sempre cria perfil **Dev**. O Admin padrão é criado via seed na inicialização: `admin@bugtracker.com` / `Admin@123`.

### Fluxo Principal

1. Usuário se cadastra e faz login (recebe JWT)
2. Admin cria um Projeto
3. Qualquer membro autenticado reporta um Bug no projeto
4. Admin atribui o bug a um Dev responsável
5. Dev atualiza o status do bug (Aberto, Em Andamento, Resolvido)
6. Sistema permite listar, filtrar, editar e excluir bugs
7. Usuários comentam nos bugs para discussão

### Severidade do Bug

- Baixa
- Média
- Alta
- Crítica

### Status do Bug

- Aberto
- EmAndamento
- Resolvido
- Fechado

---

## Arquitetura do Projeto

### Stack Tecnológica

- **Backend:** ASP.NET Core Web API (.NET 8)
- **ORM:** Entity Framework Core (EnsureCreated — sem migrations em runtime)
- **Banco de Dados:** SQL Server LocalDB
- **Autenticação:** JWT (JSON Web Token) + BCrypt para senhas
- **Documentação:** Swagger / Swashbuckle com suporte a Bearer token
- **Frontend:** HTML + Bootstrap 5 + JavaScript SPA (consumindo a API via fetch)
- **Gráficos:** Chart.js 4.4.0 (CDN)
- **Testes:** Postman

### Estrutura de Camadas

```
BugTracker/
├── Controllers/         # Endpoints HTTP
├── Models/              # Entidades do banco de dados
├── DTOs/                # Objetos de transferência de dados
├── Services/            # Regras de negócio
├── Repositories/        # Acesso ao banco de dados
├── Data/                # AppDbContext e configurações do EF Core
├── Middleware/          # ExceptionMiddleware (tratamento global de erros)
├── Migrations/          # Migrations geradas pelo EF Core
└── wwwroot/             # Interface web (index.html + js/app.js + css/style.css)
```

---

## Plano de Desenvolvimento (Ações da Atividade)

### AÇÃO 1 — Estruturação e Configuração Inicial

- [x] Criar projeto ASP.NET Core Web API
- [x] Organizar estrutura de pastas (Controllers, Models, DTOs, Services, Repositories, Middleware)
- [x] Configurar Entity Framework Core (AppDbContext com EnsureCreated)
- [x] Configurar string de conexão com o banco de dados (appsettings.json)
- [x] Configurar injeção de dependências no Program.cs

### AÇÃO 2 — CRUD e Regras de Negócio

- [x] Criar entidades: Usuario, Projeto, Bug, Comentario
- [x] Gerar e aplicar migrations (pasta Migrations com InitialCreate)
- [x] Implementar CRUD completo para cada entidade
  - GET (listar e buscar por ID)
  - POST (criar)
  - PUT (atualizar)
  - DELETE (excluir)
- [x] Aplicar validações com Data Annotations nos DTOs (`[Required]`, `[MaxLength]`, `[EmailAddress]`, `[MinLength]`)
- [x] Implementar tratamento global de erros (ExceptionMiddleware)
- [x] Padronizar retornos com códigos HTTP corretos (200, 201, 204, 400, 401, 403, 404, 500)

### AÇÃO 3 — Segurança, Documentação e Testes

- [x] Implementar autenticação com JWT (register e login via AuthService)
- [x] Configurar autorização por perfil (Admin e Dev — Viewer removido)
- [x] Proteger rotas com `[Authorize]` e `[Authorize(Roles = "Admin")]` / `[Authorize(Roles = "Admin,Dev")]`
- [x] Regra de negócio: Dev só edita bugs atribuídos a ele (verificação no BugService, retorna 403)
- [x] Configurar Swagger com suporte a JWT Bearer token
- [ ] Testar todos os endpoints no Postman

### AÇÃO 4 — Interface Web

- [x] Criar página de login integrada à API
- [x] Criar listagem dinâmica de bugs em tabela (GET)
- [x] Criar formulário de cadastro de bug (POST)
- [x] Implementar edição de bug (PUT) — apenas Admin ou Dev atribuído ao bug
- [x] Implementar exclusão de bug (DELETE) — apenas Admin
- [x] Interface responsiva com Bootstrap 5
- [x] Armazenar token JWT no localStorage e enviar no header das requisições
- [x] Dashboard com estatísticas e gráficos Chart.js (bugs por status, bugs por projeto)
- [x] Sistema de comentários por bug (thread estilo chat)
- [x] Gerenciamento de usuários pelo Admin (promover/rebaixar perfil, excluir)

### AÇÃO 5 — Entrega Final

- [ ] Repositório no GitHub com código organizado
- [x] Schema do banco funcionando (EnsureCreated + seed do admin no startup)
- [ ] Documento de arquitetura
- [ ] Evidência de funcionamento (print ou vídeo)
- [ ] Arquivo compactado: `desafio_backend_sprint3_nome_do_estudante.zip`

---

## Endpoints Implementados

### Autenticação
| Método | Rota | Descrição | Acesso |
|--------|------|-----------|--------|
| POST | /api/auth/register | Cadastrar usuário (sempre como Dev) | Público |
| POST | /api/auth/login | Login e geração de JWT | Público |

### Projetos
| Método | Rota | Descrição | Acesso |
|--------|------|-----------|--------|
| GET | /api/projetos | Listar todos | Autenticado |
| GET | /api/projetos/{id} | Buscar por ID | Autenticado |
| POST | /api/projetos | Criar projeto | Admin |
| PUT | /api/projetos/{id} | Atualizar projeto | Admin |
| DELETE | /api/projetos/{id} | Excluir projeto | Admin |

### Bugs
| Método | Rota | Descrição | Acesso |
|--------|------|-----------|--------|
| GET | /api/bugs | Listar todos | Autenticado |
| GET | /api/bugs/{id} | Buscar por ID | Autenticado |
| GET | /api/bugs/projeto/{projetoId} | Bugs por projeto | Autenticado |
| POST | /api/bugs | Reportar bug | Dev, Admin |
| PUT | /api/bugs/{id} | Atualizar bug | Dev (só o próprio), Admin |
| DELETE | /api/bugs/{id} | Excluir bug | Admin |

### Comentários
| Método | Rota | Descrição | Acesso |
|--------|------|-----------|--------|
| GET | /api/bugs/{bugId}/comentarios | Listar comentários do bug | Autenticado |
| POST | /api/bugs/{bugId}/comentarios | Postar comentário | Autenticado |

### Usuários
| Método | Rota | Descrição | Acesso |
|--------|------|-----------|--------|
| GET | /api/usuarios | Listar todos | Admin |
| GET | /api/usuarios/{id} | Buscar por ID | Admin |
| PUT | /api/usuarios/{id} | Atualizar perfil | Admin |
| DELETE | /api/usuarios/{id} | Excluir usuário | Admin |

### Dashboard
| Método | Rota | Descrição | Acesso |
|--------|------|-----------|--------|
| GET | /api/dashboard | Estatísticas gerais (totais, bugs críticos, por projeto) | Autenticado |

---

## Observações para o Claude Code

- Sempre manter a separação de responsabilidades entre camadas
- DTOs nunca devem expor a entidade diretamente (nunca retornar SenhaHash)
- Toda rota que manipula dados deve exigir autenticação, exceto /auth/register e /auth/login
- O banco usa `EnsureCreated` (não `Migrate`). Se o schema mudar, o startup detecta e recria o banco
- O startup verifica existência das tabelas `Usuarios` e `Comentarios` antes de decidir recriar
- Charts (Chart.js) devem ser destruídos e recriados a cada carga do dashboard para evitar instâncias duplicadas
- A interface web funciona com o backend em localhost (CORS configurado para AllowAnyOrigin)
