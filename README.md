# Postmodern-Technologies-in-Software-Development-
# Laboratory Work №1: Basic CI/CD Pipeline

## Description
This repository contains the implementation of Laboratory Work #1. It demonstrates the basic setup of a Continuous Integration (CI) pipeline using GitHub Actions, simple Python logic, and automated testing.

## Requirements Fulfilled
- [x] Simple code implementation (Python calculator).
- [x] 5+ automated tests using `pytest` (including positive cases and negative/exception handling for division by zero).
- [x] GitHub Actions CI pipeline (`python-tests.yml`).
- [x] Pipeline is triggered via Pull Request from a separate branch.
- [x] Code linting (`flake8`) and automated test execution.

## Project Structure
* `calculator.py` — core logic containing mathematical functions.
* `test_calculator.py` — test suite for the calculator functions.
* `.github/workflows/python-tests.yml` — configuration file for the GitHub Actions pipeline.

## How to Run Locally

1. **Install dependencies:**
   Ensure you have Python installed, then install the required packages:
   ```bash
   pip install pytest flake8
