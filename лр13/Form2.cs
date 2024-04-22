using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace лр13
{
    public partial class Form2 : Form
    {
        // Список вопросов
        private List<Question> questions;

        // Индекс текущего вопроса
        private int currentQuestionIndex = 0;

        // Счет пользователя
        private int score = 0;

        // Общее время на тестирование в секундах
        private int totalTimeInSeconds = 120;

        // Таймер
        private Timer timer;

        public Form2()
        {
            InitializeComponent();
        }

        // Загрузка формы
        private void Form2_Load(object sender, EventArgs e)
        {
            // Загрузка вопросов из файла при загрузке формы
            LoadQuestionsFromFile("questions.txt");

            // Отображение первого вопроса
            DisplayQuestion();

            // Инициализация и запуск таймера
            timer = new Timer();
            timer.Interval = 1000; // Интервал таймера - 1 секунда
            timer.Tick += Timer_Tick; // Подписываемся на событие срабатывания таймера
            timer.Start();
        }

        // Загрузка вопросов из файла
        private void LoadQuestionsFromFile(string filename)
        {
            questions = new List<Question>(); // Создаем новый список вопросов
            try
            {
                string[] lines = File.ReadAllLines(filename); // Читаем строки из файла
                foreach (string line in lines) // Перебираем каждую строку
                {
                    string[] parts = line.Split('|'); // Разделяем строку на части по символу '|'
                    if (parts.Length == 6) // Проверяем, что строка содержит правильное количество частей
                    {
                        string questionText = parts[0].Trim(); // Текст вопроса (убираем лишние пробелы)
                        List<string> answers = new List<string>(); // Создаем список для вариантов ответов
                        for (int i = 1; i < 5; i++) // Перебираем части строки, содержащие ответы
                        {
                            answers.Add(parts[i].Trim()); // Добавляем очищенные от лишних пробелов ответы в список
                        }
                        int correctAnswerIndex; // Индекс правильного ответа
                        // Пытаемся преобразовать строку в число и проверяем, что индекс в пределах от 0 до 3 (индексация начинается с нуля)
                        if (int.TryParse(parts[5].Trim(), out correctAnswerIndex) && correctAnswerIndex >= 0 && correctAnswerIndex < 4)
                        {
                            // Создаем новый вопрос и добавляем его в список
                            questions.Add(new Question(questionText, answers, correctAnswerIndex));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // В случае ошибки при чтении файла выводим сообщение об ошибке
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}");
            }
        }

        // Отображение текущего вопроса
        private void DisplayQuestion()
        {
            if (currentQuestionIndex < questions.Count)
            {
                // Если текущий индекс в пределах списка вопросов, отображаем текст вопроса и варианты ответов
                Question currentQuestion = questions[currentQuestionIndex];
                label1.Text = currentQuestion.Text;
                radioButton1.Text = currentQuestion.Answers[0];
                radioButton2.Text = currentQuestion.Answers[1];
                radioButton3.Text = currentQuestion.Answers[2];
                radioButton4.Text = currentQuestion.Answers[3];
            }
            else
            {
                // Если все вопросы пройдены, завершаем тестирование
                FinishTest();
            }
        }

        // Обработчик нажатия кнопки "Ответить"
        private void button1_Click(object sender, EventArgs e)
        {
            if (!IsAnyRadioButtonChecked())
            {
                // Если пользователь не выбрал ни один из вариантов ответа, выводим сообщение и прерываем выполнение метода
                MessageBox.Show("Пожалуйста, выберите один из вариантов ответа.");
                return;
            }

            string userAnswer = GetSelectedAnswer(); // Получаем выбранный пользователем ответ
            string correctAnswer = questions[currentQuestionIndex].Answers[questions[currentQuestionIndex].CorrectAnswerIndex]; // Получаем правильный ответ из списка вопросов

            if (userAnswer == correctAnswer)
            {
                // Если ответ пользователя совпадает с правильным ответом, увеличиваем счет
                score++;
            }
            currentQuestionIndex++; // Переходим к следующему вопросу
            DisplayQuestion(); // Отображаем следующий вопрос
        }

        // Проверка, выбран ли хотя бы один вариант ответа
        private bool IsAnyRadioButtonChecked()
        {
            return radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked;
        }

        // Получение выбранного пользователем ответа
        private string GetSelectedAnswer()
        {
            if (radioButton1.Checked) return radioButton1.Text;
            if (radioButton2.Checked) return radioButton2.Text;
            if (radioButton3.Checked) return radioButton3.Text;
            if (radioButton4.Checked) return radioButton4.Text;
            return "";
        }

        // Обновление метки с оставшимся временем
        private void Timer_Tick(object sender, EventArgs e)
        {
            totalTimeInSeconds--; // Уменьшаем время на 1 секунду
            if (totalTimeInSeconds <= 0)
            {
                // Если время вышло, останавливаем таймер и завершаем тестирование
                timer.Stop();
                FinishTest();
            }
            else
            {
                // Иначе обновляем метку с оставшимся временем
                UpdateTimerLabel();
            }
        }

        // Обновление метки с оставшимся временем
        private void UpdateTimerLabel()
        {
            // Рассчитываем минуты и секунды из общего количества секунд
            int minutes = totalTimeInSeconds / 60;
            int seconds = totalTimeInSeconds % 60;
            // Обновляем метку с оставшимся временем
            label2.Text = $"Оставшееся время: {minutes:00}:{seconds:00}";
        }

        // Завершение тестирования
        private void FinishTest()
        {
            // Останавливаем таймер
            timer.Stop();
            // Выводим сообщение о завершении теста и результате
            MessageBox.Show($"Тест завершен. Ваша оценка: {score} из {questions.Count}");
            // Закрываем форму
            this.Close();
        }
    }

    // Класс для представления вопроса
    public class Question
    {
        public string Text { get; } // Текст вопроса
        public List<string> Answers { get; } // Список вариантов ответа
        public int CorrectAnswerIndex { get; } // Индекс правильного ответа

        // Конструктор
        public Question(string text, List<string> answers, int correctAnswerIndex)
        {
            Text = text;
            Answers = answers;
            CorrectAnswerIndex = correctAnswerIndex;
        }
    }
}
