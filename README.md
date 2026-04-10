# 📌 API de Gerenciamento de Tarefas

API REST desenvolvida com **C# e ASP.NET Core** para gerenciamento de usuários, projetos e tarefas, aplicando boas práticas de desenvolvimento, **Clean Architecture** e controle de acesso baseado em perfis (RBAC).

---

## 🚀 Sobre o Projeto

Este projeto foi desenvolvido com o objetivo de demonstrar habilidades em desenvolvimento back-end utilizando .NET, com foco em:

- Organização de código  
- Separação de responsabilidades  
- Aplicação de regras de negócio  
- Construção de APIs seguras e escaláveis  

A aplicação permite gerenciar usuários, projetos e tarefas, incluindo autenticação, autorização e validações de domínio.

O sistema implementa **controle de acesso baseado em perfil (RBAC)**, com regras aplicadas na camada de serviço.

---

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture**, com separação em camadas:


### Principais conceitos aplicados

- Separação Controller → Service → Repository  
- Entidades com encapsulamento (setters privados)  
- Regras de negócio centralizadas na camada de Application  
- DTOs para entrada e saída de dados
- Baixo acoplamento entre camadas  

---

## ⚙️ Funcionalidades

### 👤 Usuários
- Cadastro de usuários  
- Autenticação (login) com geração de token JWT  
- Atualização de dados  
- Exclusão de usuários  
- Listagem de usuários  
- Controle de acesso por perfil  

### 📁 Projetos
- Criação de projetos  
- Atualização de projetos  
- Exclusão de projetos  
- Listagem de projetos  
- Associação com usuário criador  

### ✅ Tarefas
- Criação de tarefas vinculadas a projetos  
- Atribuição de tarefas a usuários  
- Atualização de tarefas  
- Exclusão de tarefas  
- Listagem de tarefas  
- Controle de status e data limite  

---

## 🔐 Segurança e Controle de Acesso

A aplicação implementa autenticação via **JWT (Json Web Token)** e controle de acesso baseado em perfis de usuário.

### 👥 Perfis de Usuário

**Administrador**
- Acesso total ao sistema  
- Pode criar, editar e remover usuários  
- Gerencia projetos e tarefas  

**Operacional**
- Pode gerenciar projetos e tarefas  
- Pode editar usuários  
- Não pode criar usuários admin  
- Não pode alterar usuários com perfil Administrador  

**Consulta**
- Acesso somente leitura  
- Pode visualizar dados (listas e busca por ID)  
- Não possui permissão para alterações  

---

## 🧠 Regras de Negócio

- Não permite cadastro com e-mail duplicado  
- Não permite projetos com nomes duplicados  
- Não permite tarefas com títulos duplicados no mesmo projeto  
- Controle de permissões baseado em perfil de usuário  

### Restrições específicas:
- Usuários operacionais não podem alterar administradores  
- Apenas administradores podem cadastrar novos usuários  

- Validação de dados antes de persistência  

---

## 🛠️ Tecnologias Utilizadas

- C#  
- .NET / ASP.NET Core  
- Entity Framework Core  
- SQL Server  
- JWT (Json Web Token)  
- xUnit  
- Moq  
- Git  

<img width="978" height="911" alt="image" src="https://github.com/user-attachments/assets/96ca87ab-31d1-4fab-aac2-5f3ca7535bde" />

