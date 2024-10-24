DROP DATABASE TestingDB;
CREATE DATABASE TestingDB;

USE TestingDB;

CREATE TABLE ResultInfo (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Student VARCHAR(255),
    Time INT,
    CAnswers INT,
    CQuestions INT,
    Mark INT,
    Date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);