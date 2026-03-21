import pytest
from app import calculate_bmi, get_bmi_status

def test_calculate_bmi():
    assert calculate_bmi(70, 1.75) == 22.86
    with pytest.raises(ValueError):
        calculate_bmi(70, 0)

def test_get_bmi_status():
    assert get_bmi_status(20.0) == "Normal"
    assert get_bmi_status(15.0) == "Underweight"
    assert get_bmi_status(30.0) == "Overweight"
