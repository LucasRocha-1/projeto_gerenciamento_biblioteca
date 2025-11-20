BEGIN TRANSACTION;
ALTER TABLE "Usuarios" ADD "Senha" TEXT NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251119011832_AddUsuarioSenha', '9.0.10');

CREATE TABLE "Emprestimos" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Emprestimos" PRIMARY KEY AUTOINCREMENT,
    "LivroId" INTEGER NOT NULL,
    "LivroTitulo" TEXT NOT NULL,
    "UsuarioId" INTEGER NOT NULL,
    "UsuarioNome" TEXT NOT NULL,
    "DataEmprestimo" TEXT NOT NULL,
    "DataDevolucao" TEXT NULL,
    "Status" TEXT NOT NULL
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251119012731_AddEmprestimosTable', '9.0.10');

ALTER TABLE "Usuarios" ADD "IsAdmin" INTEGER NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251119014351_AddIsAdminToUsuario', '9.0.10');

COMMIT;

