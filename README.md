# 🧩 Sistema de Cadastro de Usuários (WPF)

Aplicação **desktop acadêmica** desenvolvida em **C# com WPF** para **gerenciamento de usuários**.  
Implementa um CRUD completo, autenticação com **hash de senha (BCrypt)** e **arquitetura em camadas (N-Tier)** para melhor organização e manutenibilidade do código.

---

## ✨ Funcionalidades Principais

- 🔐 **Sistema de Login** – Autenticação de usuários (somente perfis “Admin” podem logar).  
- 👤 **Gerenciamento de Usuários (CRUD)**  
  - ➕ Criar novos usuários  
  - 📋 Listar e pesquisar usuários  
  - ✏️ Editar informações  
  - ❌ Excluir registros  
- 🔒 **Segurança:** Senhas hasheadas com **BCrypt**, nunca armazenadas em texto plano.  
- 🧾 **Validação de Dados:**  
  - Campos obrigatórios  
  - Validação de e-mail (Regex)  
  - Validação de CPF (algoritmo de dígitos verificadores)  
  - Verificação de duplicidade de e-mail  
- 🔍 **Busca Dinâmica:** Filtro em tempo real por nome, e-mail ou CPF.  
- 🖥️ **Interface (UI):**  
  - DataGrid para exibição  
  - Formulário único para criação/edição  
  - Máscara automática de CPF  

---

## 🛠️ Tecnologias Utilizadas

| Tecnologia | Descrição |
|-------------|------------|
| 🧠 **.NET (C#)** | Linguagem principal |
| 🖼️ **WPF (Windows Presentation Foundation)** | Interface gráfica |
| 🐬 **MySQL** | Banco de dados relacional |
| 🔗 **MySql.Data** | Driver de conexão ADO.NET |
| 🧩 **BCrypt.Net** | Hash e verificação de senhas |

---

## 🏛️ Arquitetura do Projeto

O projeto adota a arquitetura **N-Tier**, separando claramente as responsabilidades:

### 🧱 **Camadas Principais**
- **Models** (`/Models`)  
  Estruturas de dados e entidades (`Pessoa`, `Usuario`, `UserRole`).

- **View** (`/`)  
  Interface visual (XAML) e lógica da interface (`MainWindow.xaml.cs`, `TelaDeCadastro.xaml.cs`).

- **Controller** (`/Controllers`)  
  Ponte entre a UI e os serviços, aplicando o padrão **façade**.

- **Services** (`/Services`)  
  Lógica de negócio, validações, regras e abstrações de segurança (`IUsuarioService`, `UsuarioService`, `BCryptPasswordHasher`, `IPasswordHasher`).

- **DAO (Data Access Object)** (`/DAO`)  
  Acesso direto ao banco via interfaces (`IUsuarioDAO`, `MySqlUsuarioDAO`).

💡 **Injeção de Dependência:**  
Feita manualmente na camada de View, tornando o código **desacoplado e testável**.

---

## 🚀 Como Executar o Projeto

### 1️⃣ Pré-requisitos
- Visual Studio 2019+  
- .NET SDK  
- Servidor MySQL (XAMPP, WAMP ou MySQL Server)

### 2️⃣ Configuração do Banco de Dados
Execute o script SQL abaixo no seu MySQL:

```sql
CREATE DATABASE IF NOT EXISTS cadastro_db;
USE cadastro_db;

CREATE TABLE IF NOT EXISTS UserRole (
    Id INT PRIMARY KEY,
    Nome VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS Pessoa (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(255) NOT NULL,
    DataNascimento DATE NOT NULL,
    Cpf VARCHAR(14) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS Usuario (
    PessoaId INT PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Senha VARCHAR(255) NOT NULL,
    RoleId INT NOT NULL,
    FOREIGN KEY (PessoaId) REFERENCES Pessoa(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES UserRole(Id)
);

INSERT IGNORE INTO UserRole (Id, Nome) VALUES (1, 'Comum'), (2, 'Admin');

-- Inserir usuário Admin padrão
INSERT INTO Pessoa (Nome, DataNascimento, Cpf) 
VALUES ('Administrador', '2000-01-01', '123.456.789-00');

-- Senha: admin (hasheada com BCrypt, cost 12)
INSERT INTO Usuario (PessoaId, Email, Senha, RoleId) 
VALUES (LAST_INSERT_ID(), 'admin@admin.com', '$2a$12$kCzFxAoSZSyNCc9JIfswT.zY7JujnHDs0T/g.VhfZ1YlPZVbBeuZG', 2);
```

### 3️⃣ Configurar a String de Conexão
Edite em:  
`SistemaCadastroUsuarios/Services/MySqlUsuarioDAO.cs`

```csharp
private readonly string _connectionString = "Server=localhost;Database=cadastro_db;Uid=root;Pwd=;";
```

Atualize `Server`, `Uid` e `Pwd` conforme sua configuração.

### 4️⃣ Executar
- Abra o `.sln` no Visual Studio  
- Compile (Ctrl+Shift+B)  
- Inicie (F5)  
- Faça login com:  
  - **Email:** admin@admin.com  
  - **Senha:** admin  

---

## 🧪 Estrutura Visual (Exemplo)

```
SistemaCadastroUsuarios/
├── Controllers/
│   └── UsuarioController.cs
├── Models/
│   ├── Pessoa.cs
│   ├── Usuario.cs
│   └── UserRole.cs
├── Services/
│   ├── IUsuarioService.cs
│   ├── UsuarioService.cs
│   ├── IPasswordHasher.cs
│   └── BCryptPasswordHasher.cs
├── DAO/
│   ├── IUsuarioDAO.cs
│   └── MySqlUsuarioDAO.cs
└── Views/
    ├── MainWindow.xaml
    └── TelaDeCadastro.xaml
```

---

## 📚 Aprendizados e Propósito

Este projeto foi desenvolvido como parte de um **trabalho acadêmico** com foco em:
- Boas práticas de arquitetura de software  
- Aplicação de princípios de **separação de responsabilidades**  
- Segurança na manipulação de dados sensíveis  
- Desenvolvimento desktop com WPF  

---

## 🧑‍💻 Autor

**Felipe Machiaveli**  

---

> *"Código limpo é aquele que você pode entender mesmo depois de meses sem vê-lo."*  
> — Robert C. Martin
