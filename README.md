Sistema de Cadastro de Usuários (WPF)
Este é um projeto acadêmico de uma aplicação desktop (WPF) em C# para gerenciamento de usuários. A aplicação implementa um CRUD (Create, Read, Update, Delete) completo, sistema de login com hash de senha e uma arquitetura em camadas (N-Tier) para separação de responsabilidades.

O sistema foi projetado para consumir um banco de dados MySQL, sendo capaz de compartilhar a mesma base de dados com outras aplicações (como um backend em PHP).

✨ Funcionalidades Principais
Sistema de Login: Autenticação de usuários para acesso ao sistema. Apenas usuários com perfil "Admin" (RoleId = 2) podem logar.

Gerenciamento de Usuários (CRUD):

Create: Adicionar novos usuários (com dados de Pessoa e Usuário).

Read: Listar e pesquisar todos os usuários cadastrados.

Update: Editar informações de usuários existentes.

Delete: Remover usuários do sistema.

Segurança: As senhas nunca são salvas em texto plano. Elas são hasheadas usando BCrypt antes de serem salvas no banco.

Validação de Dados: Validação robusta no lado do serviço (UsuarioService) para:

Campos obrigatórios.

Formato de e-mail (Regex).

Validação de CPF (algoritmo de dígitos verificadores).

Verificação de e-mail duplicado.

Busca / Filtro: Pesquisa dinâmica de usuários no DataGrid por nome, e-mail ou CPF.

Interface (UI):

Uso de DataGrid para exibição dos dados.

Formulário único para criação e edição (o estado do formulário muda dependendo se um usuário está selecionado).

Máscara de formatação automática para o campo CPF.

🛠️ Tecnologias Utilizadas
.NET (C#)

WPF (Windows Presentation Foundation): Para a interface gráfica.

MySQL: Banco de dados relacional.

MySql.Data: Driver de conexão ADO.NET para MySQL.

BCrypt.Net: Biblioteca para hashing e verificação de senhas.

🏛️ Arquitetura do Projeto
O projeto segue uma arquitetura em camadas (N-Tier) para separar a lógica de apresentação, a lógica de negócio e o acesso a dados.

Models (/Models)

Representam as entidades do sistema.

Pessoa.cs: Classe base com dados pessoais (Id, Nome, DataNascimento, Cpf).

Usuario.cs: Herda de Pessoa e adiciona dados de autenticação (Email, Senha, UserRoleId).

UserRole.cs: Representa os papéis/permissões (ex: Admin, Comum).

View (/)

Responsável pela UI (as janelas .xaml) e pelo code-behind (.xaml.cs).

MainWindow.xaml.cs: Tela de Login.

TelaDeCadastro.xaml.cs: Tela principal de CRUD (listagem, cadastro, edição).

Controller (/Controllers)

Atua como um intermediário (façade) entre a View e os Serviços. A View só conhece o Controller.

UsuarioController.cs: Recebe solicitações da View (ex: FazerLogin) e as repassa para o IUsuarioService.

Services (/Services)

Contém toda a lógica de negócio do sistema.

IUsuarioService.cs: Interface que define o "contrato" da lógica de usuário (o que o sistema pode fazer).

UsuarioService.cs: Implementação do contrato. É aqui que ocorrem as validações (CPF, email), verificações de duplicidade e o hashing de senhas.

IPasswordHasher.cs / BcryptPasswordHasher.cs: Abstração para o serviço de hashing, permitindo que o BCrypt seja trocado futuramente sem quebrar o UsuarioService.

DAO (Data Access Object) (/Services)

Camada responsável exclusivamente pela comunicação com o banco de dados.

IUsuarioDAO.cs: Interface que define o "contrato" de acesso a dados (ex: Adicionar, Listar, GetPorEmail).

MySqlUsuarioDAO.cs: Implementação que contém as queries SQL (INSERT, UPDATE, SELECT, DELETE) e a lógica de transação para o MySQL.

InMemoryUsuarioService.cs: (Nota: Este arquivo parece ser uma implementação alternativa de IUsuarioService para testes em memória, não um DAO).

Injeção de Dependência (ID)
O sistema utiliza Injeção de Dependência manual na "raiz de composição" (as janelas da View). As Views (MainWindow e TelaDeCadastro) são responsáveis por criar as instâncias concretas (MySqlUsuarioDAO, BcryptPasswordHasher, UsuarioService) e "injetá-las" no construtor do UsuarioController.

Isso permite que o UsuarioController e o UsuarioService dependam apenas de interfaces (IUsuarioDAO, IUsuarioService), tornando o código desacoplado e fácil de testar.

🚀 Como Executar
1. Pré-requisitos
Visual Studio 2019 (ou mais recente)

.NET SDK (compatível com o projeto)

Um servidor MySQL (como MySQL Community Server, XAMPP, WAMP).

2. Configuração do Banco de Dados
O MySqlUsuarioDAO espera a seguinte estrutura de banco de dados. Execute o script abaixo no seu MySQL:

SQL

-- 1. Crie o banco de dados
CREATE DATABASE IF NOT EXISTS cadastro_db;
USE cadastro_db;

-- 2. Tabela de Permissões (Roles)
CREATE TABLE IF NOT EXISTS UserRole (
    Id INT PRIMARY KEY,
    Nome VARCHAR(50) NOT NULL
);

-- 3. Tabela Base (Pessoa)
CREATE TABLE IF NOT EXISTS Pessoa (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(255) NOT NULL,
    DataNascimento DATE NOT NULL,
    Cpf VARCHAR(14) NOT NULL UNIQUE
);

-- 4. Tabela de Usuários (herda de Pessoa)
CREATE TABLE IF NOT EXISTS Usuario (
    PessoaId INT PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Senha VARCHAR(255) NOT NULL,
    RoleId INT NOT NULL,
    
    FOREIGN KEY (PessoaId) REFERENCES Pessoa(Id)
        ON DELETE CASCADE, -- Importante: Deleta o Usuário se a Pessoa for deletada
    FOREIGN KEY (RoleId) REFERENCES UserRole(Id)
);

-- 5. Inserir as permissões padrão
-- (O sistema espera RoleId=2 para Admin)
INSERT IGNORE INTO UserRole (Id, Nome) VALUES (1, 'Comum');
INSERT IGNORE INTO UserRole (Id, Nome) VALUES (2, 'Admin');

-- 6. (Opcional) Criar um usuário Admin para testes
/* A senha é 'admin'. O BCrypt irá gerar um hash.
   Este script SÓ PODE SER RODADO UMA VEZ.
*/
/*
INSERT INTO Pessoa (Nome, DataNascimento, Cpf) 
VALUES ('Administrador', '2000-01-01', '123.456.789-00');

-- O hash abaixo é para a senha 'admin' (gerado com BCrypt, cost 12)
INSERT INTO Usuario (PessoaId, Email, Senha, RoleId) 
VALUES (LAST_INSERT_ID(), 'admin@admin.com', '$2a$12$Yj.eX/39m.kPKsL8f7d9lOaYd2.1lI6lZY.9.tH7Y.i5.K/S9eS.S', 2);
*/
3. Configurar String de Conexão
A string de conexão com o banco de dados está hardcoded (fixa no código). Você deve alterá-la para que aponte para o seu servidor MySQL.

Abra o arquivo: SistemaCadastroUsuarios/Services/MySqlUsuarioDAO.cs

Localize a linha:

C#

private readonly string _connectionString = "Server=localhost;Database=cadastro_db;Uid=root;Pwd=;"; //
Altere Server, Database, Uid (usuário) e Pwd (senha) conforme a sua configuração local.

4. Rodar a Aplicação
Abra o arquivo .sln no Visual Studio.

Compile a solução (Build > Build Solution ou Ctrl+Shift+B).

Inicie o projeto (Debug > Start Debugging ou F5).

A tela de login (MainWindow) será aberta. Use as credenciais de Admin (ex: admin@admin.com / admin se você usou o script opcional) para entrar.
