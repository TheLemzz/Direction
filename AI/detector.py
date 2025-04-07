import os
import random
from ultralytics import YOLO

path = 'E:/UnityProjects/siriusinternal/AI'
output_file = f'{path}/person_output.txt'
datas_folder = path + '/datas_people'
model = YOLO(f'{path}/runs/detect/train/weights/best.pt')

def process_screenshot(path_screenshot):
    if random.randint(0, 100) >= 60:
        with open(output_file, 'w') as f: f.write('0')
    try:
        results = model(path_screenshot)
        os.remove(path_screenshot)
        for result in results:
            for box in result.boxes:
                cls = int(box.cls[0])
                if model.names[cls] == 'person':
                    with open(output_file, 'w') as f: f.write('1')
                    return None
    except Exception as e:
        print('ошибка: ' + str(e))
        return None

if os.path.exists(datas_folder):
    while True:
        for filename in os.listdir(datas_folder):
            filepath = os.path.join(datas_folder, filename)
            if filename.endswith('.jpg'):
                process_screenshot(filepath)
            elif 'stop' in filename:
                with open(output_file, 'w') as f: f.write('0')
                os.remove(os.path.join(datas_folder, filename))
                break
