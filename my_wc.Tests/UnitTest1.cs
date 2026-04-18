using NUnit.Framework;
using System.IO;
using my_wc;

namespace my_wc.Tests
{
    public class WcTests
    {
        [Test]
        public void Run_WithStdinAndOptionN_ReturnsSuccess()
        {
            // Arrange
            var input = new StringReader("hello\nworld\n");
            var output = new StringWriter();
            var error = new StringWriter();

            // Act - рахуємо тільки слова (-w)
            int exitCode = Program.Run(new[] { "-w" }, input, output, error);

            // Assert
            Assert.That(exitCode, Is.EqualTo(0)); // Успіх
            Assert.That(output.ToString().Trim(), Is.EqualTo("2")); // 2 слова
            Assert.That(error.ToString(), Is.Empty);
        }

        [Test]
        public void Run_WithUnknownOption_ReturnsCode2()
        {
            // Arrange
            var input = new StringReader("");
            var output = new StringWriter();
            var error = new StringWriter();

            // Act
            int exitCode = Program.Run(new[] { "--unknown-option" }, input, output, error);

            // Assert
            Assert.That(exitCode, Is.EqualTo(2)); // Неправильні аргументи
            Assert.That(output.ToString(), Is.Empty);
            Assert.That(error.ToString(), Does.Contain("Unknown option").IgnoreCase);
        }

        [Test]
        public void Run_WithNonExistentFile_ReturnsCode1()
        {
            // Arrange
            var output = new StringWriter();
            var error = new StringWriter();

            // Act
            int exitCode = Program.Run(new[] { "does_not_exist.txt" }, new StringReader(""), output, error);

            // Assert
            Assert.That(exitCode, Is.EqualTo(1)); // Часткова помилка
            Assert.That(error.ToString(), Does.Contain("No such file or directory"));
        }
    }
}