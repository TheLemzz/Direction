from ultralytics import YOLO
import os
import random

path = 'E:/UnityProjects/siriusinternal/AI'
model = YOLO(f'{path}/runs_crack/detect/train/weights/best.pt')
data_file = f'{path}/data.txt'
output_file = f'{path}/output.txt'
datas_folder = path + '/datas'

def append_number_to_file(number, filename=data_file):
    if number is None:
        return
    try:
        with open(filename, 'a') as f: f.write(str(min(1.0, number)) + '\n')
    except Exception as e:
        print(f'Ошибка при добавлении числа в файл: {e}')


def process_screenshot(path_screenshot):
    try:
        results = model(path_screenshot)
        os.remove(path_screenshot)
        crack_count = 0
        for result in results:
            for _ in result.boxes:
                crack_count += 1

        confidence = max(0.00, 1.00 - (crack_count * 0.05))
        with open(data_file, 'a') as f: f.write(str(confidence) + '\n')
        return confidence

    except Exception as e:
        print('Road.py: ошибка: ' + str(e))
        return None


def calculate_and_write_average():
    avg = calculate_average()
    if avg is not None:
        try:
            rounded_avg = round(avg, 2)
            with open(output_file, 'w') as f:
                f.write(str(rounded_avg))
        except Exception:
            pass

def calculate_average():
    try:
        numbers = []
        with open(data_file, 'r') as f:
            for line in f:
                try:
                    numbers.append(float(line.strip()))
                except ValueError:
                    pass
        if not numbers: return None
        return sum(numbers) / len(numbers)
    except FileNotFoundError:
        print(f'Road.py: Ошибка: файл {data_file} не найден.')
        return None
    except Exception as e:
        print(f'Road.py: Ошибка при вычислении среднего значения: {str(e)}')
        return None


if os.path.exists(datas_folder):
    while True:
        for filename in os.listdir(datas_folder):
            filepath = os.path.join(datas_folder, filename)
            if filename.endswith('.jpg'):
                 append_number_to_file(process_screenshot(filepath))
                 if random.randint(0, 100) <= 10: calculate_and_write_average()
            elif 'stop' in filename:
                os.remove(os.path.join(datas_folder, filename))
                break


