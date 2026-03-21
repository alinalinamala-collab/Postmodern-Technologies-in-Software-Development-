def calculate_bmi(weight_kg, height_m):
    """Розраховує індекс маси тіла."""
    if height_m <= 0:
        raise ValueError("Зріст має бути більшим за нуль (уникнення ділення на 0)")
    if weight_kg <= 0:
        raise ValueError("Вага має бути більшою за нуль")
    return round(weight_kg / (height_m ** 2), 2)

def get_bmi_status(bmi):
    """Визначає категорію за показником ІМТ."""
    if bmi <= 0:
        raise ValueError("ІМТ не може бути нульовим або від'ємним")
    if bmi < 18.5:
        return "Underweight"
    if bmi < 25.0:
        return "Normal"
    return "Overweight"
    
if __name__ == "__main__":
    weight = 70
    height = 1.75
    bmi = calculate_bmi(weight, height)
    status = get_bmi_status(bmi)
    print(f"Вага: {weight} кг, Зріст: {height} м")
    print(f"Ваш ІМТ: {bmi} ({status})")
