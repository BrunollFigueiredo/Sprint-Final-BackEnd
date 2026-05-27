# GameBugTracker — API REST

Sistema de rastreamento de bugs voltado para equipes de desenvolvimento de jogos. Permite registrar problemas encontrados durante o desenvolvimento, acompanhar o ciclo de vida de cada bug, atribuir responsáveis e colaborar com comentários.

---

## 1. Descrição do Projeto

O GameBugTracker é uma API REST completa que oferece:

- Cadastro e autenticação de usuários com perfis de acesso (Admin e Dev)
- Gerenciamento de projetos (jogos em desenvolvimento)
- Registro, atribuição e acompanhamento de bugs com severidade e status
- Sistema de comentários por bug (discussão em thread)
- Tags por departamento (Arte, Áudio, Gameplay, UI/UX, Performance)
- Passos de reprodução interativos (checklist)
- Histórico de mudanças de status por bug (timeline)
- Upload de mídia (prints e vídeos) via Supabase Storage
- Dashboard com estatísticas e gráficos interativos
- Interface web responsiva SPA consumindo a API via fetch

---

## 2. Tecnologias Utilizadas

- **Linguagem:** C# 12 / .NET 8
- **Framework:** ASP.NET Core 8 Web API
- **ORM:** Entity Framework Core 8 com Pomelo.EntityFrameworkCore.MySql
- **Banco de Dados:** MySQL (hospedado no Railway)
- **Segurança:** JWT (JSON Web Token) + BCrypt para hash de senhas
- **Documentação de API:** Swagger / Swashbuckle com suporte a Bearer token
- **Armazenamento de Mídia:** Supabase Storage (REST API)
- **Frontend:** HTML5 + Bootstrap 5.3 + JavaScript (SPA) + Chart.js 4.4.0

---

## 3. Instruções de Execução

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- Git

### Passo a passo

1. **Clone o repositório**
   ```bash
   git clone https://github.com/<seu-usuario>/Sprint-Final-BackEnd.git
   cd Sprint-Final-BackEnd/BugTracker
   ```

2. **Configure o `appsettings.json`**

   Copie o arquivo `appsettings.Example.json` para `appsettings.json`:
   ```bash
   cp appsettings.Example.json appsettings.json
   ```
   Preencha os valores com as credenciais fornecidas no arquivo ZIP de entrega (as strings de conexão e chaves já estão preenchidas no arquivo incluído no ZIP).

3. **Restaure as dependências**
   ```bash
   dotnet restore
   ```

4. **Execute a aplicação**
   ```bash
   dotnet run
   ```
   O banco de dados é criado automaticamente na primeira execução (`EnsureCreated`). Um usuário Admin padrão é inserido via seed: `admin@bugtracker.com` / `Admin@123`.

5. **Acesse a interface e o Swagger**
   - Interface web: [http://localhost:5000](http://localhost:5000)
   - Swagger UI: [http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## 4. Endpoints da API

### Autenticação
| Método | Rota | Descrição | Acesso |
|--------|------|-----------|--------|
| POST | `/api/auth/register` | Cadastrar usuário (cria como Dev) | Público |
| POST | `/api/auth/login` | Login e geração de JWT | Público |

### Projetos
| Método | Rota | Descrição | Acesso |
|--------|------|-----------|--------|
| GET | `/api/projetos` | Listar todos os projetos | Autenticado |
| GET | `/api/projetos/{id}` | Buscar projeto por ID | Autenticado |
| POST | `/api/projetos` | Criar projeto | Admin |
| PUT | `/api/projetos/{id}` | Atualizar projeto | Admin |
| DELETE | `/api/projetos/{id}` | Excluir projeto | Admin |

### Bugs
| Método | Rota | Descrição | Acesso |
|--------|------|-----------|--------|
| GET | `/api/bugs` | Listar todos os bugs | Autenticado |
| GET | `/api/bugs/{id}` | Buscar bug por ID | Autenticado |
| GET | `/api/bugs/projeto/{projetoId}` | Bugs de um projeto | Autenticado |
| POST | `/api/bugs` | Reportar bug | Autenticado |
| PUT | `/api/bugs/{id}` | Atualizar bug | Admin ou Dev atribuído |
| DELETE | `/api/bugs/{id}` | Excluir bug | Admin |

### Comentários
| Método | Rota | Descrição | Acesso |
|--------|------|-----------|--------|
| GET | `/api/bugs/{bugId}/comentarios` | Listar comentários do bug | Autenticado |
| POST | `/api/bugs/{bugId}/comentarios` | Postar comentário | Autenticado |

### Mídia
| Método | Rota | Descrição | Acesso |
|--------|------|-----------|--------|
| POST | `/api/bugs/{bugId}/media` | Upload de imagem/vídeo | Autenticado |
| DELETE | `/api/bugs/{bugId}/media/{mediaId}` | Excluir mídia | Admin ou quem enviou |

### Tags
| Método | Rota | Descrição | Acesso |
|--------|------|-----------|--------|
| GET | `/api/tags` | Listar todas as tags | Autenticado |
| POST | `/api/tags` | Criar tag | Admin |
| DELETE | `/api/tags/{id}` | Excluir tag | Admin |

### Usuários
| Método | Rota | Descrição | Acesso |
|--------|------|-----------|--------|
| GET | `/api/usuarios` | Listar usuários | Admin |
| GET | `/api/usuarios/{id}` | Buscar usuário por ID | Admin |
| PUT | `/api/usuarios/{id}` | Atualizar perfil/cargo | Admin |
| DELETE | `/api/usuarios/{id}` | Excluir usuário | Admin |

### Dashboard
| Método | Rota | Descrição | Acesso |
|--------|------|-----------|--------|
| GET | `/api/dashboard` | Estatísticas gerais | Autenticado |

---

## 5. Estrutura do Projeto

```
BugTracker/
├── Controllers/        # Endpoints HTTP (AuthController, BugsController, etc.)
├── Models/             # Entidades do banco (Bug, Usuario, Projeto, Comentario, Tag, BugMedia, BugHistorico)
├── DTOs/               # Objetos de transferência de dados (sem expor SenhaHash)
├── Services/           # Regras de negócio (BugService, AuthService, TagService, etc.)
├── Repositories/       # Acesso ao banco via EF Core
├── Data/               # AppDbContext + configurações de relacionamentos
├── Middleware/         # ExceptionMiddleware (tratamento global de erros)
├── Migrations/         # Migrations geradas pelo EF Core
└── wwwroot/            # Interface web (index.html + js/app.js + css/style.css)
```

---

## 6. Perfis de Acesso

| Perfil | Permissões |
|--------|-----------|
| **Admin** | Acesso total: criar projetos, gerenciar usuários, atribuir e excluir bugs, gerenciar tags |
| **Dev** | Reportar bugs, atualizar status dos bugs atribuídos a ele, postar comentários |

> Registro público sempre cria perfil **Dev**. O Admin padrão é criado no seed: `admin@bugtracker.com` / `Admin@123`.

---

## 7. Demonstração

- **Aplicação em produção:** [https://sprint-final-backend-production.up.railway.app](https://sprint-final-backend-production.up.railway.app)
- **Swagger em produção:** [https://sprint-final-backend-production.up.railway.app/swagger](https://sprint-final-backend-production.up.railway.app/swagger)
