from __future__ import annotations

import os
import time
from multiprocessing import Process

from locust import HttpUser, between, task

process = None


class WebsiteUser(HttpUser):
    wait_time = between(1, 3)  # Time between consecutive requests in seconds

    @task
    def telemetry_endpoint(self):
        response = self.client.get("/telemetry?ID=1")
        if response.status_code != 200:
            print(f"Telemetry request failed with status code: {response.status_code}")

    @task
    def point_endpoint(self):
        payload = {
            "coordinate": {"x": 1.0, "y": 2.0, "z": 3.0},
            "rotation": {"p": 0.0, "y": 90.0, "r": 0.0}
        }
        response = self.client.put("/point?ID=1", json=payload)
        if response.status_code != 200:
            print(f"Point request failed with status code: {response.status_code}")

    @task
    def download_image_endpoint(self):
        payload = {"some_key": "some_value"}
        response = self.client.post("/downloadImage", json=payload)
        if response.status_code != 200:
            print(f"Download image request failed with status code: {response.status_code}")
