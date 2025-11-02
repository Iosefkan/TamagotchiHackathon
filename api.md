- GET /labubu/health
  - Возвращает 
  ```json
  {
      "health": 1, 
      "weight": 1, 
      "status": 1,
      "message": null | "message"
  }
  ```
- GET /labubu/getCurrentClothes
  - Вовзвращает 
  ```json
  [
      {
      "body_part_id": 1,
      "cloth_id": 1,
      "url": "minio_url"
      }
  ]
  ```
- POST /labubu/setClothes
  - Принимает 
  ```json
  {
      "body_part_id": 1, 
      "cloth_id": 1
  }
  ```
- POST /labubu/action
  - Принимает
  ```json
  {
      "action_type": 12
  }
  ```
  - Возвращает
  ```json
  {
      "health": 1, 
      "weight": 1, 
      "status": 1
  }
  ```
---
- GET /clothes?body_part_id=1
  - Возвращает 
  ```json
  {
      "clothes_id": 1,
      "name": "name",
      "url": "minio_url"
  }
  ```
---
- GET /achievements
  - Возвращает 
  ```json
  [
      {
          "achievement_id": 1,
          "name": "name",
          "desc": "desc",
          "url": "url",
          "is_completed": true
      }
  ]
  ```
---
- POST /game/start
  - Принимает
  ```json
  {
      "game_id": 1
  }
  ```
- POST /game/end
  - Принимает
  ```json
  {
      "game_id": 1,
      "score": 100,
      "specific_data": {}
  }
  ```
  - Возвращает
  ```json
  {
      "award": ?
  }
  ```
---
- GET /user/energy
  - Возвращает
  ```json
  {
      "energy": 100
  }
  ```
- GET /user/info
  - Возвращает
  ```json
  {
    "login": "login",
    "email": "email",
    "banks": [
        {
            "bank_id": 1,
            "bank_name": "name",
            "balance": 12.12
        }
    ]
  }
  ```