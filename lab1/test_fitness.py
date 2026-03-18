import pytest
from fitness import calculate_bmi, get_bmi_status

# 1. Позитивний тест: правильний розрахунок
def test_calculate_bmi_normal():
    assert calculate_bmi(65, 1.70) == 22.49

# 2. Позитивний тест: правильний статус
def test_get_bmi_status_normal():
    assert get_bmi_status(22.49) == "Normal"

# 3. Негативний тест: перевірка ділення на 0 (нульовий зріст)
def test_calculate_bmi_zero_height():
    with pytest.raises(ValueError, match="уникнення ділення на 0"):
        calculate_bmi(65, 0)

# 4. Негативний тест: від'ємна вага
def test_calculate_bmi_negative_weight():
    with pytest.raises(ValueError, match="більшою за нуль"):
        calculate_bmi(-10, 1.70)

# 5. Негативний тест: некоректний ІМТ для статусу
def test_get_bmi_status_invalid():
    with pytest.raises(ValueError):
        get_bmi_status(0)
