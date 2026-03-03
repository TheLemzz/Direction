import zmq
import threading
import json
import time


class UnityConnector:
    def __init__(self, script_name, pub_port=5555, pull_port=5556):
        self.script_name = script_name
        self.context = zmq.Context()
        self.running = True

        self.sub_socket = self.context.socket(zmq.SUB)
        self.sub_socket.connect(f"tcp://localhost:{pub_port}")

        self.sub_socket.setsockopt_string(zmq.SUBSCRIBE, self.script_name)
        self.sub_socket.setsockopt_string(zmq.SUBSCRIBE, "ALL")

        self.push_socket = self.context.socket(zmq.PUSH)
        self.push_socket.connect(f"tcp://localhost:{pull_port}")

        self.command_handlers = {}

        self.listener_thread = threading.Thread(target=self._listen_loop)
        self.listener_thread.daemon = True
        self.listener_thread.start()

    def _listen_loop(self):
        while self.running:
            try:
                if self.sub_socket.poll(100):
                    topic = self.sub_socket.recv_string()
                    command = self.sub_socket.recv_string()
                    data_str = self.sub_socket.recv_string()

                    data = json.loads(data_str)

                    if command == "EXIT":
                        print(f"[{self.script_name}] Received EXIT signal.")
                        self.running = False
                        exit()

                    if command in self.command_handlers:
                        try:
                            self.command_handlers[command](data)
                        except Exception as e:
                            print(f"Error handling {command}: {e}")
                    else:
                        print(f"[{self.script_name}] Unknown command: {command}")
            except zmq.ZMQError:
                break

    def on_command(self, command_name, callback):
        """Регистрация обработчика команды от Unity"""
        self.command_handlers[command_name] = callback

    def send_event(self, event_name, data_dict):
        """Отправка события в Unity"""
        payload = {
            "event": event_name,
            "sender": self.script_name,
            "data": data_dict
        }
        self.push_socket.send_string(json.dumps(payload))

    def wait(self):
        while self.running:
            time.sleep(0.1)

        self.sub_socket.close()
        self.push_socket.close()
        self.context.term()
        print(f"[{self.script_name}] Disconnected.")