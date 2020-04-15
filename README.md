Location based Sns using AR 
------------------------------------
there are three functions

(1) AR Message 

(2) AR Navigation

(3) AR POI

These functions are implemented using latitude and longitude.

representative algorithm is the haversine algorithm.

The haversine formula determines the great-circle distance between two points on a sphere given their longitudes and latitudes.

-------------------------------------
기능
-------------------------------------
(1) AR 메시지 : 자기가 원하는 위치에 텍스트를 담은 말풍선을 띄울 수 있음

(2) AR 네비게이션 : 출발지와 도착지를 선택하면 카메라에 LinePath를 띄어줌

(3) AR POI : 사용자의 주변의 정보들을 불러와 증강현실 오브젝트로 주변 정보들을 띄어줌 

구현 방법
-------------------------------------
C#과 Unity 그리고 AR Foundation을 이용하여 AR 기능들을 모두 구현 

AR Message와 AR Navigation의 기본이 되는 방법은 두개의 위도와 경도를 가지는 포인트 사이의 거리를 구하여 오브젝트의 위치를 지정해주는 것

위도와 경도를 가지는 포인트 사이의 거리를 측정하기 위하여 Haversine formula를 이용함 

그리고 AR Message 같은 경우는 사용자 등록한 메시지의 데이터를 계속 유지해야하므로 Firebase를 연동하여 Storage로 이용함

AR 네비게이션 : Tmap api를 이용해 경로가 꺾이는 곳마다 좌표를 불러와 그 곳에 오브젝트를 배치 시킨 후, Line Rederer를 이용해 선으로 이어줌

AR POI : Naver api를 이용해 주변 정보를 불러와 그 좌표마다 오브젝트를 배치 시키고, 거리를 계산해 거리도 사용자에게 보여줌 

AR 메시지 : 파이어베이스에 다양한 사용자들이 배치시킨 메시지들을 저장시킴 


Haversine formula*
-------------------------------------
Haversine formula란 구에서 두가지 점을 찍었을 때 거리를 구할 수 있는 알고리즘  

완벽하게 정확한 방법은 아니지만, Haversine formula를 이용해 충분히 구현할 수 있다고 판단해 선택

Firebase 
-------------------------------------
Realtime DB를 이용하여 다양한 커뮤니케이션 구현 

Youtube 
-------------------------------------
https://www.youtube.com/watch?v=E1drjy37nwc&t=4s

