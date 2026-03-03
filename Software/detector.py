# тестовый скрипт

import argparse
import time
from direction_connector import UnityConnector

parser = argparse.ArgumentParser()
parser.add_argument("--name", default="detector.py", help="Unique script name for ZMQ topic")
args, unknown = parser.parse_known_args()
connector = UnityConnector(script_name=args.name)

val = 1

def handle_set_threshold(data):
    global val
    val = data.get("value", 0.5)
    print(f"Threshold updated to {val}")


connector.on_command("SET_THRESHOLD", handle_set_threshold)

try:
    print("Detector started working...")
    while connector.running:
        time.sleep(2)

        connector.send_event("PERSON_DETECTED", {"count": 1, "pos": [10, 0, 5], "threshold": val})

except KeyboardInterrupt:
    pass