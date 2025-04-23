# Refatoracao do Sistema de Gerenciamento de Biblioteca

## Parte 1: Problemas Identificados (Violações de SOLID e Clean Code)

### 1. SRP - Single Responsibility Principle

- **Onde:** Classe `GerenciadorBiblioteca`
- **Problema:** Acumula múltiplas responsabilidades (cadastro, empréstimos, devolução, cálculo de multa, envio de notificações).
- **Justificativa:** Complica a manutenção e viola a coesão da classe.

### 2. OCP - Open/Closed Principle

- **Onde:** Métodos `AdicionarUsuario` e `RealizarEmprestimo`
- **Problema:** Adição de novos tipos de notificação exige modificação direta do código.
- **Justificativa:** O sistema não está aberto para extensão sem modificação do código existente.

### 3. DIP - Dependency Inversion Principle

- **Onde:** Uso direto de `EnviarEmail()` e `EnviarSMS()`
- **Problema:** Alto acoplamento entre lógica de negócio e implementações concretas de notificação.
- **Justificativa:** Dificulta testes, reaproveitamento e manutenção.

### 4. Clean Code - Nomes e responsabilidades

- **Onde:** Métodos com nomes genéricos ou enganosos (ex: `AdicionarUsuario` que também envia e-mail).
- **Problema:** Métodos fazem mais do que seus nomes sugerem.
- **Justificativa:** Dificulta compreensão rápida do código.

### 5. ISP - Interface Segregation Principle (implícito)

- **Onde:** Ausência de interfaces para abstração de notificações.
- **Problema:** Código depende de detalhes e não de abstrações.
- **Justificativa:** Reduz flexibilidade e reutilização de componentes.

---

## Parte 2: Solução Refatorada

### 1. Separacão de responsabilidades

- `BibliotecaService`: Gerencia livros e usuários
- `EmprestimoService`: Lida com empréstimos e devoluções
- `MultaService`: Calcula multas
- `Notificadores`: `EmailNotificador` e `SmsNotificador` cuidam apenas de notificações

### 2. Uso de Interfaces (DIP e OCP)

- Interface `INotificador` criada
- `EmprestimoService` depende de `INotificador`, permitindo diferentes formas de notificação

### 3. Código limpo e bem nomeado

- Funções pequenas, com nomes que indicam claramente sua responsabilidade
- Separadas em arquivos por tipo de serviço ou modelo

### 4. Fácil extensão (OCP)

- Novos tipos de notificação podem ser adicionados sem alterar código existente, apenas implementando `INotificador`

### 5. Pronto para testes

- Uso de dependências via interfaces torna o sistema mais testável e modular

---
