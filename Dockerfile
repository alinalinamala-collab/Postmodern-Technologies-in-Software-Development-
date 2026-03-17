FROM python:3.10-slim

WORKDIR /app

# Копіюємо список залежностей і встановлюємо їх
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Копіюємо код додатку та тестів
COPY . .

# Команда за замовчуванням (запуск додатку)
CMD ["python", "app.py"]
