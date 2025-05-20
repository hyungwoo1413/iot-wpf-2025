## 파이썬 MQTT Publish
# paho-mqtt 라이브러리 설치
# pip install paho-mqtt

import paho.mqtt.client as mqtt
import json
import datetime as dt
import uuid
from collections import OrderedDict
import random
import time

PUB_ID = 'IOT77' # 본인 ip 주소
BROKER = '210.119.12.77' # 본인 ip
PORT = 1883
TOPIC = 'smarthome/77/topic' # publish/subscribe 에서 사용할 토픽(핵심)
COLORS = ['RED', 'ORANGE', 'YELLOW', 'GREEN', 'BLUE', 'NAVY', 'PURPLE']
COUNT = 0

# [Fake] 센서 설정
SENSOR1 = "온습도센서 셋팅"; PIN1 = 5
SENSOR2 = "포토센서 셋팅"; PIN2 = 7
SENSOR3 = "워터드롭센서 셋팅"; PIN3 = 9
SENSOR4 = "인체감지센서 셋팅"; PIN4 = 11


# 연결 콜백
def on_connect(client, userdata, flags, reason_code, properties=None):
    print(f'Connected with reason code : {reason_code}')

def on_publish(client, userdata, mid):
    print(f'Message published mid : {mid}')

try:
    client = mqtt.Client(client_id=PUB_ID, protocol=mqtt.MQTTv5)
    client.on_connect = on_connect
    client.on_publish = on_publish

    client.connect(BROKER, PORT)
    client.loop_start()

    while True:
        # 퍼블리시
        currtime = dt.datetime.now()
        selected = random.choice(COLORS)
        temp = random.uniform(20.0, 29.9) # 실제로는 센서에서 값을 받음
        humid = random.uniform(40.0, 65.9)
        rain = random.randint(0, 1)
        detect = random.randint(0, 1)
        photo = random.randint(50, 255)

        COUNT += 1

        raw_data = OrderedDict()
        raw_data['PUB_ID'] = PUB_ID
        raw_data['COUNT'] = COUNT
        raw_data['SENSING_DT'] = currtime.strftime(f'%Y-%m-%d %H:%M:%S')
        raw_data['TEMP'] = f'{temp:0.1f}'
        raw_data['HUMID'] = f'{humid:0.1f}'
        raw_data['LIGHT'] = 1 if photo >= 200 else 0
        raw_data['HUMAN'] = detect
        
        ## OrderedDict -> json 타입으로 변경
        pub_data = json.dumps(raw_data, ensure_ascii=False, indent='\t')
        
        client.publish(TOPIC, payload = pub_data, qos=1)
        time.sleep(1)

except Exception as ex:
    print(f'Error raised : {ex}')
except KeyboardInterrupt:
    print('MQTT 전송중단')
    client.loop_stop()
    client.disconnect()