Location based Sns using AR 
------------------------------------
there are three functions

(1) AR Message 

(2) AR Navigation

(3) AR POI
```
These functions are implemented using latitude and longitude. 

representative algorithm is the haversine algorithm. 

The haversine formula determines the great-circle distance between two points on a sphere given their longitudes and latitudes.
```
<br/>
<br/>

기능
-------------------------------------
1. AR Message : 자기가 원하는 위치에 텍스트를 담은 말풍선을 띄울 수 있음

2. AR Navigation : 출발지와 도착지를 선택하면 카메라에 LinePath를 띄어줌

3. AR POI : 사용자의 주변의 정보들을 불러와 증강현실 오브젝트로 주변 정보들을 띄어줌 

구현 방법
-------------------------------------
1. Unity 그리고 `AR Foundation`을 이용하여 AR 기능들을 모두 구현 

2. `AR Message`와 `AR Navigation`의 기본이 되는 방법은 두개의 위도와 경도를 가지는 포인트 사이의 거리를 구하여 오브젝트의 위치를 지정해주는 것

3. 위도와 경도를 가지는 포인트 사이의 거리를 측정하기 위하여 `Haversine formula`를 이용함 

4. DB 및 서버 : `Firebase`

5. AR Navigation : `Tmap api`를 이용해 경로가 꺾이는 곳마다 좌표를 불러와 그 곳에 게임 오브젝트를 배치 시킨 후, `Line Rederer`를 이용해 선으로 이어줌

6. AR POI : `Naver api`를 이용해 주변 장소들을 불러와 그 좌표마다 오브젝트를 배치 시켜 사용자에게 장소의 방향 정보를 알려줌

7. AR Message : `FireBase`에 다양한 사용자들이 배치시킨 메시지들의 위치 정보와 내용을 저장시킨 후 사용자들에게 보여줌


Haversine formula*
-------------------------------------
1. `Haversine formula`란 구에서 두가지 점을 찍었을 때 거리를 구할 수 있는 알고리즘  
2. 100%의 정확도를 보여주진 않지만, `Haversine formula`를 이용해 충분히 구현할 수 있다고 판단해 선택

<br/>
<br/>


참조
-------------------------------------
[안드로이드 SNS 앱 + AR 모듈](https://github.com/dudcheol/LittletigersInit) <br/>
[유튜브](https://www.youtube.com/watch?v=E1drjy37nwc&t=4s) 


Image
-------------------------------------
![DDubuk](https://user-images.githubusercontent.com/44944031/87563447-2e5a1280-c6fa-11ea-9b57-a4d0c108d616.png)


