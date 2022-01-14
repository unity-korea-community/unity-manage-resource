# Initial page

## 소개

이 프로젝트는 Unity Custom Package 프로젝트에서 자주 보이는 패턴을 Template 프로젝트로 만들었습니다.

## repository generate 된 뒤 해야 할 일

* github action에 환경변수 변경
* github secret 추가
* workspace - unity project 이름 변경
* workspace - unity package 세팅 
  * Example
    * com.unko.\[PackageName\].editor
    * com.unko.\[PackageName\].editor.tests

## 들어있는 것

### master branch

unity package manager에서 git link를 통해 바로 설치할 수 있는 unity custom package 형식의 branch입니다.

```text
<root>
  ├── package.json
  ├── README.md
  ├── CHANGELOG.md
  ├── LICENSE.md
  ├── Editor
  │   ├── Unity.[YourPackageName].Editor.asmdef
  │   └── EditorExample.cs
  ├── Runtime
  │   ├── Unity.[YourPackageName].asmdef
  │   └── RuntimeExample.cs
  ├── Tests
  │   ├── Editor
  │   │   ├── Unity.[YourPackageName].Editor.Tests.asmdef
  │   │   └── EditorExampleTest.cs
  │   └── Runtime
  │        ├── Unity.[YourPackageName].Tests.asmdef
  │        └── RuntimeExampleTest.cs
  └── Documentation~
       └── [YourPackageName].md
```

유니티 공식 메뉴얼 중 패키지 레이아웃을 따릅니다.

[https://docs.unity3d.com/kr/2019.4/Manual/cus-layout.html](https://docs.unity3d.com/kr/2019.4/Manual/cus-layout.html)

#### master - github action

* **copy-to-workspace.yml**
  * push시 workspace branch에 copy합니다.
* **manual-copy.yml**
  * 수동으로 workspace branch에 copy합니다.



### workspace branch

해당 패키지를 작업할 Unity Project입니다.

```text
<root>
├─.github
│  └─workflows
├─Assets
│  └─[YourPackageName]
│      ├─Editor
│      ├─Runtime
│      └─Tests
├─Packages
├─ProjectSettings
```

#### workspace - github action

* **manual-copy.yml**
  * 수동으로 workspace branch에 copy합니다.
* **unittest-and-upload-master.yml**
  * unity - unit test를 한 뒤 성공하면 master branch에 copy합니다.

