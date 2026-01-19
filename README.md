# TodoList API – Estudo de API REST com ASP.NET Core (.NET 10)

## 📌 Objetivo do Projeto
Projeto desenvolvido como **estudo prático de ASP.NET Core (.NET 10 LTS)**, com foco em APIs REST seguras, organização de código e **consolidação de fundamentos da plataforma .NET moderna**.

O objetivo é demonstrar domínio progressivo do ecossistema .NET por meio da aplicação de conceitos sólidos de backend, como autenticação, middlewares, validação, acesso a dados e documentação automatizada.

---

## 🧩 Descrição
API focada em performance, simplicidade e clareza de responsabilidades, desenvolvida como prova de conceito.
O projeto aplica princípios de separação de responsabilidades inspirados em **Clean Architecture**, segurança baseada em **JWT com validação de sessão ativa** e documentação automatizada via **Scalar**.

---

## 🛠️ Stack Tecnológica
- **Runtime**: .NET 10 LTS (C# 12+)
- **Persistência**: MySQL com Entity Framework Core (Code First)
- **Segurança**: JWT Bearer + controle de sessão ativa via middleware
- **Validação**: FluentValidation integrado ao pipeline de model binding
- **Documentação**: OpenAPI gerado via `Microsoft.AspNetCore.OpenApi` e renderizado com Scalar
- **Utilitários**:
  - DotNetEnv para gestão de variáveis de ambiente
  - BCrypt.Net-Next para hashing de senhas

---

## 🧱 Arquitetura e Features Implementadas
- [x] **Tratamento Global de Erros**  
  Implementação de `IExceptionHandler` para interceptação de exceções e retorno de payload JSON consistente, evitando vazamento de detalhes internos em produção.

- [x] **Options Pattern**  
  Centralização e validação de configurações sensíveis (JWT) durante a inicialização da aplicação.

- [x] **Infraestrutura OpenAPI Customizada**  
  Uso de `DocumentTransformer` para configuração explícita de esquemas de segurança JWT no pipeline OpenAPI do .NET 10.

- [x] **Validação de Sessão Ativa**  
  Middleware posicionado entre Autenticação e Autorização para validação de tokens com base em sessões persistidas (JTI), permitindo revogação de acesso em tempo real.

- [x] **Módulo de Identidade**  
  Registro, autenticação JWT com sessões persistidas, gestão do perfil do usuário autenticado, troca de senha com invalidação de sessões e endpoint `/me` para verificação de contexto do usuário.

- [ ] **Gestão de Tarefas**  
  Endpoints CRUD de tarefas vinculadas estritamente ao usuário autenticado.

---

## 🔐 Fluxo de Autenticação
1. Usuário realiza login e recebe um **JWT**.
2. O token contém um **JTI (JWT ID)** único.
3. A sessão é persistida no banco de dados.
4. A cada requisição autenticada:
   - O JWT é validado.
   - O middleware verifica se a sessão ainda está ativa.
5. Tokens revogados ou sessões expiradas resultam em **HTTP 401 (Unauthorized)**.

---

## 📄 Endpoints (exemplos)
- `POST /api/auth/register` – Registro de usuário
- `POST /api/auth/login` – Autenticação e emissão de JWT
- `POST /api/auth/logout` – Revogação da sessão ativa (autenticado)
- `GET /api/users/{id}` – Consulta de usuário (autenticado)

> A lista completa de endpoints pode ser consultada via Scalar.

---

## 🚀 Instruções de Execução

### 1. Pré-requisitos
- .NET SDK 10 instalado
- MySQL em execução

### 2. Configuração de Ambiente
O projeto utiliza variáveis de ambiente centralizadas na **raiz do repositório**
(antes da pasta `src/`).

Utilize o arquivo `.env.example` como referência e crie um arquivo `.env`:

```bash
cp .env.example .env
```

Exemplo de variáveis:

```env
DB_HOST=localhost
DB_PORT=3306
DB_NAME=todo_list
DB_USER=root
DB_PASSWORD=senha

Jwt__Key=chave-secreta
Jwt__Issuer=TodoList.API
Jwt__Audience=TodoList.Client
Jwt__ExpireMinutes=60
```

### 3. Migrações de Banco de Dados
```bash
dotnet ef database update
```

### 4. Execução do Projeto
```bash
dotnet watch run
```

### 5. Documentação da API
A interface de documentação está disponível em ambiente de **Development**:

```
/scalar
```

---

## 🐳 Docker (Opcional)
O repositório inclui arquivos auxiliares (`compose.yaml`) para execução do ambiente via Docker, facilitando a configuração de dependências como o banco de dados.

---

## ✅ Padrões de Qualidade
- **Tipagem Forte**: Uso de DTOs com validação explícita e inicialização segura (`required` / `init` quando aplicável).
- **Segurança**:
  - Hashing de senhas com BCrypt
  - Política de Zero-Leak para stack traces em produção
- **Extensibilidade**:
  - Configuração de OpenAPI desacoplada via `DocumentTransformer`
  - Middlewares customizados para regras de negócio transversais

---

## 📚 Observações Finais
Este projeto é voltado para **consolidação de fundamentos e exploração prática** da plataforma .NET moderna, servindo como base para evolução contínua e aprofundamento em cenários reais de produção.
