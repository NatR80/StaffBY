USE StaffBYDB;
GO

-- Удаляем старых пользователей (если есть)
DELETE FROM Users WHERE Username IN ('admin', 'hr_user', 'economist', 'accountant', 'hr', 'eco', 'acc');
GO

-- Создаем новых пользователей с короткими логинами и паролями
-- Пароль для всех: admin (хеш: 8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918)

-- Администратор (Role = 0) - полный доступ
INSERT INTO Users (Username, PasswordHash, Role, IsActive, CreatedAt) 
VALUES ('admin', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 0, 1, GETDATE());

-- Кадровик (Role = 1) - сотрудники, отпуска, табель, документы, отчеты, архив
INSERT INTO Users (Username, PasswordHash, Role, IsActive, CreatedAt) 
VALUES ('hr', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 1, 1, GETDATE());

-- Экономист (Role = 2) - штатное расписание, начисление ЗП
INSERT INTO Users (Username, PasswordHash, Role, IsActive, CreatedAt) 
VALUES ('eco', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 2, 1, GETDATE());

-- Бухгалтер (Role = 3) - выплата ЗП
INSERT INTO Users (Username, PasswordHash, Role, IsActive, CreatedAt) 
VALUES ('acc', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 3, 1, GETDATE());
GO

-- Проверяем результат
SELECT Id, Username, Role FROM Users;
GO