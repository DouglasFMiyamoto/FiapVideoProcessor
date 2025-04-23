# 🎞️ FIAP Video Processor

Sistema de processamento de vídeos com extração de imagens e entrega em arquivos `.zip`, desenvolvido com foco em escalabilidade, mensageria, autenticação, qualidade de software e boas práticas de arquitetura.

---

## 📚 Introdução

Este projeto foi desenvolvido para a empresa **FIAP X**, que apresentou uma primeira versão simples do sistema durante uma rodada de investimentos. O objetivo era extrair imagens de vídeos enviados e disponibilizá-las para download em um arquivo `.zip`. Com a aprovação dos investidores, a nova versão foi desenvolvida com foco em robustez e escalabilidade.

---

## 💡 Funcionalidades

- ✅ Upload de vídeos para processamento
- ✅ Processamento assíncrono com suporte a múltiplos vídeos simultâneos
- ✅ Download do arquivo `.zip` contendo as imagens extraídas
- ✅ Autenticação de usuários via login e senha
- ✅ Listagem de status dos vídeos por usuário
- ✅ Notificação em caso de falha (via e-mail ou canal alternativo)
- ✅ Painel para acompanhar status dos vídeos
- ✅ Suporte a picos de carga com uso de mensageria (SQS)
- ✅ Escalabilidade via contêineres Docker
- ✅ Pipeline de CI/CD com testes automatizados

---

## 🧱 Arquitetura

- **.NET 8** com [FastEndpoints](https://fast-endpoints.com/) para construção de APIs modernas e performáticas
- Arquitetura de **Microsserviços**
- Separação de responsabilidades com **Clean Architecture / Hexagonal**
- **Mensageria** com Amazon SQS (via LocalStack em desenvolvimento)
- **Persistência** com PostgreSQL e armazenamento de arquivos com S3 (ou equivalente local)
- **Docker + Docker Compose** para orquestração local
- **Testes automatizados** com cobertura mínima de 80%
- **CI/CD** com GitHub Actions
---
![Arquitetura](https://github.com/user-attachments/assets/3eab8973-bd22-4e54-84f8-e162bbc1aa8d)
---

## 🗃️ Estrutura do Projeto

```bash
fiap-video-processor/
├── src/
│   ├── VideoProcessor.API        # Interface de entrada (FastEndpoints)
│   ├── VideoProcessor.Core       # Regras de negócio (UseCases, Entidades)
│   ├── VideoProcessor.Infrastructure # Conexões com SQS, S3, PostgreSQL
│   └── VideoProcessor.Worker     # Worker para processamento assíncrono
├── tests/
│   └── VideoProcessor.Tests      # Testes unitários e de integração
├── docker-compose.yml            # Orquestração de serviços
├── README.md                     # Este arquivo
└── .github/workflows             # CI/CD com GitHub Actions


