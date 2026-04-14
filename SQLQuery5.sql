USE StaffBYDB;
GO

-- Добавим подразделения
INSERT INTO Departments (Name, Description, IsActive, CreatedAt) 
VALUES 
(N'Руководство', N'Высшее руководство', 1, GETDATE()),
(N'Бухгалтерия', N'Финансовый учет', 1, GETDATE()),
(N'IT отдел', N'Информационные технологии', 1, GETDATE()),
(N'Отдел продаж', N'Продажи и маркетинг', 1, GETDATE()),
(N'Отдел кадров', N'Управление персоналом', 1, GETDATE());
GO

-- Добавим должности
INSERT INTO Positions (Title, BaseSalary) 
VALUES 
(N'Директор', 5000),
(N'Главный бухгалтер', 3500),
(N'Бухгалтер', 2000),
(N'Инженер-программист', 2500),
(N'Менеджер', 2200),
(N'Специалист', 1800),
(N'Экономист', 2000),
(N'Юрист', 2400),
(N'Секретарь', 1400);
GO

-- Добавим сотрудников
DECLARE @DeptRukovodstvo INT = (SELECT Id FROM Departments WHERE Name = N'Руководство');
DECLARE @DeptBuhgalteria INT = (SELECT Id FROM Departments WHERE Name = N'Бухгалтерия');
DECLARE @DeptIT INT = (SELECT Id FROM Departments WHERE Name = N'IT отдел');
DECLARE @DeptSales INT = (SELECT Id FROM Departments WHERE Name = N'Отдел продаж');
DECLARE @DeptHR INT = (SELECT Id FROM Departments WHERE Name = N'Отдел кадров');

DECLARE @PosDirector INT = (SELECT Id FROM Positions WHERE Title = N'Директор');
DECLARE @PosGlavBuh INT = (SELECT Id FROM Positions WHERE Title = N'Главный бухгалтер');
DECLARE @PosBuh INT = (SELECT Id FROM Positions WHERE Title = N'Бухгалтер');
DECLARE @PosProgrammer INT = (SELECT Id FROM Positions WHERE Title = N'Инженер-программист');
DECLARE @PosManager INT = (SELECT Id FROM Positions WHERE Title = N'Менеджер');
DECLARE @PosSpecialist INT = (SELECT Id FROM Positions WHERE Title = N'Специалист');

-- Вставляем сотрудников (с N перед строками)
INSERT INTO Employees (LastName, FirstName, Patronymic, PersonalNumber, BirthDate, BirthPlace, Citizenship, 
                       DepartmentId, PositionId, IsArchived, CreatedAt, IsMale, Phone, Email, HomeAddress)
VALUES 
(N'Иванов', N'Иван', N'Иванович', '001', '1980-05-15', N'г.Минск', N'Беларусь', 
 @DeptRukovodstvo, @PosDirector, 0, GETDATE(), 1, N'+375291234567', N'ivanov@company.by', N'ул.Ленина 10'),

(N'Петрова', N'Анна', N'Сергеевна', '002', '1985-08-22', N'г.Минск', N'Беларусь', 
 @DeptBuhgalteria, @PosGlavBuh, 0, GETDATE(), 0, N'+375292345678', N'petrova@company.by', N'ул.Пушкина 5'),

(N'Сидоров', N'Петр', N'Алексеевич', '003', '1990-03-10', N'г.Гомель', N'Беларусь', 
 @DeptIT, @PosProgrammer, 0, GETDATE(), 1, N'+375293456789', N'sidorov@company.by', N'пр.Независимости 25'),

(N'Козлова', N'Елена', N'Владимировна', '004', '1988-07-19', N'г.Минск', N'Беларусь', 
 @DeptBuhgalteria, @PosBuh, 0, GETDATE(), 0, N'+375294567890', N'kozlova@company.by', N'ул.Сурганова 15'),

(N'Морозов', N'Дмитрий', N'Алексеевич', '005', '1992-11-25', N'г.Брест', N'Беларусь', 
 @DeptSales, @PosManager, 0, GETDATE(), 1, N'+375295678901', N'morozov@company.by', N'ул.Московская 8'),

(N'Новикова', N'Ольга', N'Петровна', '006', '1995-02-14', N'г.Минск', N'Беларусь', 
 @DeptHR, @PosSpecialist, 0, GETDATE(), 0, N'+375296789012', N'novikova@company.by', N'ул.Янки Купалы 12'),

(N'Соколов', N'Андрей', N'Викторович', '007', '1987-09-30', N'г.Витебск', N'Беларусь', 
 @DeptIT, @PosProgrammer, 0, GETDATE(), 1, N'+375297890123', N'sokolov@company.by', N'ул.Октябрьская 3'),

(N'Михайлова', N'Татьяна', N'Игоревна', '008', '1991-04-18', N'г.Минск', N'Беларусь', 
 @DeptHR, @PosSpecialist, 0, GETDATE(), 0, N'+375298901234', N'mikhailova@company.by', N'ул.Калиновского 7'),

(N'Волков', N'Сергей', N'Александрович', '009', '1983-12-05', N'г.Могилев', N'Беларусь', 
 @DeptSales, @PosManager, 0, GETDATE(), 1, N'+375299012345', N'volkov@company.by', N'ул.Ленинградская 20'),

(N'Зайцева', N'Мария', N'Дмитриевна', '010', '1993-06-22', N'г.Минск', N'Беларусь', 
 @DeptIT, @PosProgrammer, 0, GETDATE(), 0, N'+375291234568', N'zaitseva@company.by', N'ул.Горького 45');
GO

-- Проверим результат (теперь должно быть нормально)
SELECT Id, LastName, FirstName, Patronymic, PersonalNumber, 
       (SELECT Name FROM Departments WHERE Id = Employees.DepartmentId) as Department,
       (SELECT Title FROM Positions WHERE Id = Employees.PositionId) as Position
FROM Employees;