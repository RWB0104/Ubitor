# Ubitor

<p align="center">
	<img src="https://user-images.githubusercontent.com/50317129/91200590-4f227880-e73a-11ea-893c-0419bdfabadc.jpg" width="256" height="256" alt="EasyReport" title="EasyReport">
</p>

네트워크 다운로더

# INFO

개발언어 : `C#`

#### 지원 OS
+ Microsoft **Windows10**

#### Framework
+ **.NET Framework 4.7.2**
+ **MahApps.Metro**
+ **MahApps.Metro.IconPacks**
+ **Fody**

<br />
<br />

# RELEASE

## Ubitor 1.0.0 RELEASE

2020.08.26 화요일 Update

+ HTTP 통신에 기반한 다운로더
+ 대부분의 HTTP Method 지원
+ Referer 임의 지정 가능
+ User-Agent 임의 지정 가능 (기본값 Mozila)
+ DLL 포함 빌드

<br />
<br />

# 사용 방법

<p align="center">
	<img src="https://user-images.githubusercontent.com/50317129/91201720-c60c4100-e73b-11ea-8e5c-cb7289ab66db.png" width="50%" alt="EasyReport" title="EasyReport">
</p>

1. ComboBox를 이용하여 HTTP Method를 선택함. (ex. GET, POST...)
2. URL을 입력함 (HTTP, HTTPS 프로토콜 지원)
3. Referer 입력 (선택사항)
4. User-Agent 입력 (선택사항, 기본값은 Mozila)
5. [SEND]를 누르고 파일 경로, 이름, 확장자를 지정하면 해당 파일로 응답 내용이 다운로드 됨.
6. 중단하고 싶을 경우, 다운로드 시 표시되는 [CANCEL]을 누름.
7. 다운로드가 중단되어도, 이미 다운로드된 파일은 삭제되지 않음.

<br />
<br />

# 여담

C# 가지고 놀다가 Metro UI라는 프레임워크를 발견했다. WinForm의 지루한 UI에 질린 차에 웹처럼 디자인을 미려하게 바꿀 수 있다는 게 신선했다.
<br />
하지만 WinForm의 한계로 인해 아쉬워하다가 WPF를 알게됬고, 그냥 WinForm과 비슷하겠거니 했는데, 진정한 C#은 WPF가 아니였나 싶다. 확실히 컨트롤 디자인이 훨씬 자유롭다. 살짝 어렵긴 한데, 핵심 소스는 C#이랑 동일하고, XAML? 얘는 HTML 다루는 느낌이다. 거기다가 MahApps.Metro라는 강력한 UI Framework가 있어서 이전에 C#으로 만든 그 어떤 프로그램보다 미려한 UI를 구성할 수 있었다.
<br />
MahApps.Metro도 한 번 다뤄볼 겸 시험삼아 네트워크 응답을 다운로드 하는 프로그램을 만들었다. 그냥 범용적이기도 하고, 나중에 쓸모도 있을 것 같다... 아마도?