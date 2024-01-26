## Introduction
The project represents the dockerized web app that receives a JSON containing the list of passengers and returns the airplaine layout.

## How to run it
Start the project!

Perform a post with a json int the following format:
"type": 0 means Adult, "type": 1 means: Enfant

```javascript
[
    {"id": 1, "type": 0, "age": 35, "familyName": "A", "needs2Places": false},
    {"id": 2, "type": 0, "age": 32, "familyName": "A", "needs2Places": false},
    {"id": 3, "type": 1, "age": 7,"familyName": "A", "needs2Places": false},
    {"id": 4, "type": 1, "age": 4, "familyName": "A", "needs2Places": false},
    {"id": 5,"type": 0, "age": 45,"familyName": "B", "needs2Places": false},
    {"id": 6, "type": 1, "age": 10, "familyName": "B", "needs2Places": false}
    {"id": 148, "type": 1, "age": 17, "familyName": null, "needs2Places": false}	
]
```
									
The response will be either validation error or the airplaine layout.

I have included a persons.json file in the repo for testing purposes.
