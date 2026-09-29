using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Collections;
using UnityEngine;


#region 유틸리티 : CPrint
/*
▶ 유틸리티 : CPrint

- 구조화를 시키고 싶다. → 출력에 관한 것을...

- 프린트는 콘솔 프로젝트에서 사용한 것처럼 출력 규칙을 유니티스럽게 바꾼 버전

ㆍ유니티에서 무언가를 만드려고 할때...

C# → Main

유니티 → 유니티 생성주기에 올리고 대신 호출을 하게 한다.

유니티에서는 .... Global Using을 권장하지 않는다.
 ㄴ 이놈 자체가 자동화가 완벽하게 안된다.

ㆍ유니티에서 전역(느낌)스럽게 사용하고 싶다?

1. 글로벌 네임스페이스 → Static
 ㄴ 별다른 using 없어도 잘 돌아감

2. 네임 스페이스는 유지하되 풀네임 호출로 통일한다.
 ㄴ Ex : Common.CPrint.Title("")
 ㄴ 어디 소속인지 명확하다.

3. using static


▶ CPrint 업그레이드


*/
#endregion



public static class CPrint
{
    // 옵션
    // 스위치

    // if(!Enable) return; 로 사용할 수 있다.
    public static bool Enable = true;

    //서식 태그 (콘솔 컬러 태그)
    public static bool EnableRichText = true;

    // 들여 쓰기 → 당연히 가독성을 위해 사용 → 출력 앞에 공백을 붙여 구조적으로 분리하기가 좋다.
    //  ㄴ 묶음 / 같은 종류 → 트리 구조처럼 만들겠다. → 가독성
    private static int _indentLevel = 0;
    private const int INDENT_SPACES = 2;


    // HashSet : 중복을 허용하지 않고 고유한 요소만 저장한다. (자료구조)
    //  ㄴ 일반적으로 시간복잡도 O(1) 수준을 보인다.
    // readonly : 참조를 다른 HashSet으로 바꾸지 않도록 잠구겠다.
    private static readonly HashSet<string> _onceSet = new HashSet<string>();

    /*
    
    ▶ readonly
     ㄴ 런타입

    - 한 번 정해진 이후에는 다시 대입하지 못하게 막는다.
     ㄴ 초기화 → 선언부에서 하거나 / 생성자에서 하거나
     
     - MonoBehaviour 때문에 생성자를 직접적으로 사용하는 경우는 C# 대비 많지 않다.


    ▶ HashSet

    - 컬렉션 클래스 → 해시 테이블 기반 → 데이터 구조
      ㄴ 중복되지 않은 요소들의 모임을 관리 / 이럴 경우 최적화 되어 있고 탐색이 빠름 → 추가 및 삭제도 가능

    ▷ 해시 테이블
     ㄴ 키 / 값이 쌍으로 데이터를 저장하는 자료구조
     - 키를 이용해 (해시 함수) 특정 인덱스로 접근 (혹은 변환) → 데이터 저장 → 삽입 / 삭제 / 검색이 빠르다.
     
     
    ㆍ내부 동작

    1. 해시 함수

    - 값을 → 해시 코드로(정수) 바꾼다.
     ㄴ 같은 값이면 같은 해시 코드가 나오는게 이 자료구조의 목표
     ㄴ 1(해시 코드) 1(해시 코드)

    2. 버킷
    - 해시 코드를 기준으로 저장 위치(버킷)를 고른다.
     ㄴ 대충 "해시코드 % 버킷개수" 같은 방식으로 인덱스를 걸정한다고 생각하면 된다.

    3. 충돌
    - 서로 다른 값인데 해시 코드가 겹칠 수 있다. (+ 버킷)
     ㄴ 버킷 안에서 추가 비교 등을 수행해서 진짜 같은 값인지 확인
     ㄴ 예외처리에서 반드시 막아줘야 하는 부분
    - HsshSet → 해시 + 실제 비교를 같이 쓴다.

    4. 재해싱
    - 안에 요소가 많아지면 → 버킷이 뻑뻑해 진다. → 성능이 떨어질 수밖에 없음.
    - 더 큰 테이블(버킷)을 만들고 다시 배치한다. → 재해싱 ※ 이미 할당받은 메모리에서 바뀌지 않고 재배치를 수행

    ※ 중복 없이 저장 + 빠른 컨테이너    

    ※ 내부는 해시로 위치 찾고 → 충돌은 비교로 해결 → 많아지면 재해싱

     */

    private static string Indent
    {
        get
        {   // 레벨 * 공백수
            // 로그에서 활용
            return new string(' ', _indentLevel * INDENT_SPACES);
        }
    }

    // 단계별 출력을 줄 맞춰서 읽기 쉽게 만든다.
    public static void IndentPush()
    {
        // 단계 올리기
        _indentLevel++;
    }

    public static void IndentPop()
    {
        // 단계 내리기
        _indentLevel--;

        if(_indentLevel < 0)
        {
            _indentLevel = 0;
        }

    }
    public static void IndentReset()
    {
        _indentLevel = 0;
    }

    private enum ELogKind
    {
        Log,
        Warn,
        Error,
        Success
    }


    // 출력 포맷 관리를 위해
    // 들여쓰기 / 접두사 / 리치 텍스트 → Kind 분류
    private static void Emit(ELogKind kind, string msg, string tag = null, string colorHex = null)
    {
        // 지금까지 만든 문자열을 콘솔로 내보내는 출력 코어

        // 색상값 → 헥사를 쓰는 이유 → 가장 범용적인 방식이기 때문 (문자열로 색을 표현하기에 가장 무난)
        // ㄴ 1. 표준이고 무난
        // ㄴ 2. 문자열 → 로그 포맷을 만들 때 바로 끼워넣기 좋음
        // ㄴ 3. 16진수 → 압축이 잘됨 (RGB)
        // - RGB(0, 0, 0) → 0 ~ 255 → FF = 255 / 00 = 0

        if (!Enable)
        {
            return;
        }

        // 접두사 만들기 → Tag가 있으면 해당되는 프리픽스를 만든다.
        // 단, Tag가 null / 빈 문자열이면 접두사 없이 msg만 출력

        string prefix = string.Empty;

        if (!string.IsNullOrEmpty(tag))
        {
            // t / colorHex → tag 부분만 색을 입히겠다. → 가독성
            if (EnableRichText && !string.IsNullOrEmpty(colorHex))
            {
                // IsNullOrEmpty(s)
                // ㄴ 문자열이 쓸 수 있는 값인지 검사한다.
                // ㄴ s == Null / s == "" → t
                prefix = $"<color = {colorHex}> [{tag}] </color>";
            }
            else
            {   
                // (리치 텍스트를 사용안하거나) 색상이 없다면 기본 형태로 만든다.
                // 공백이 있어야 msg랑 안 붙는다.
                prefix = $"[{tag}]"; 
            }
        }

        string final = $"{Indent}{prefix}{msg}";


        switch (kind)
        {
            case ELogKind.Log:

            case ELogKind.Success:
                Debug.Log(final);
                break;
            case ELogKind.Warn:
                Debug.LogWarning(final);
                break;
            case ELogKind.Error:
                Debug.LogError(final);
                break;
            
          
        }
    }



    // Title / Section
    public static void Title(string title, char lineCh = '=')
    {
        Line(lineCh);
        Emit(ELogKind.Log, title);
        Line(lineCh);
    }
    public static void Section(string section, char lineCh = '=')
    {
        Emit(ELogKind.Log, section);
        Line(lineCh);
    }


    
    // Line / Blank
    // 구분선을 상황에 맞게 바꿀 수 있도록 문자 / 길이를 옵션으로 준 것
    //  ㄴ 고정 형식이 아니기에 상황에 맞춰 사용하면 효율 ↑
    public static void Line(char ch = '-', int count = 10)
    {
        Emit(ELogKind.Log, new string(ch, count));
    }
    public static void Blank(int lines = 1)
    {
        // 콘솔에 빈줆만 추가 (접두사 / 인덴트/ 색 / 태그 전부 사용 안함)
        // Emit 들어가면 인덴트 붙어서 애매해짐

        if (!Enable)
        {
            return;
        }


        // 빈 줄 여러줄
        if(lines <= 0)
        {
            return;
        }

        Debug.Log(new string('\n', lines));
    }

    // Log / Warn / Error

    public static void Log(string msg)
    {
        Emit(ELogKind.Log,msg);
    }
    public static void Warn(string msg) 
    {
        // 주황 느낌
        Emit(ELogKind.Warn,msg, "WARN", "#FF9100");
    }
    public static void Error(string msg)
    {
        // 빨간 계열
        Emit(ELogKind.Error, msg, "Error", "#FF1744");
    }
    public static void Success(string msg)
    {
        // 빨간 계열
        Emit(ELogKind.Success, msg, "OK", "#00C853");
    }

    // ㄴ 경고와 달리 빨간색 → 남발 금지 → 진짜 필요한 것에만 사용

    // Assert → 강력한 예외 방어
    public static void Assert(bool condition, string msg)
    {
        // if문이 많이 빠질 수 있음 → 매번 if문 하는 것보다 Assert로 확인하는게 더 깔끔

        if (condition)
        {
            return;
        }

        Error($"[ASSERT] {msg}");
       
    }

    public static void CheckNull(object obj, string msg)
    {
        if (obj != null)
        {
            return;
        }

        Warn($"[NULL] : {msg}");
    }

    public static T Ref<T>(T obj, string msg) where T : class
    {
        if (obj == null)
        {
            Warn($"[NULL] {msg}");
        }

        return obj;
    }
    /*
     왜 T냐?
      ㄴ 한 줄로 끝내려고. 
      ㄴ 동작 시키기 위해서

    _rb = CPrint.Ref(GetComponent<RigidBody>, "...");
    _tf = CPrint.Ref(GetComponent<Transform>, "...");

    -필수 컴포넌트 + 바로 대입을 해주기 위함

    ㆍ제네릭 → 가볍게
     
    - 제네릭은 → 타입을 나중에 정하는 설계법
     ㄴ 호출할 때 타입이 결정된다.

    - 템플릿 / 제네릭 → 비슷한 얘기다.
     ㄴ 클래스나 함수를 정의할 때 타입을 지정하지 않고 구현할 수 있는 매커니즘
     
     ㆍ T
    - 타입 자리 (타입 변수)
    - 제네릭은 <T>와 같은 제네릭 타입을 명시함으로서의 정의 가능
    - 객체지향 특징 + 원칙
      ㄴ 추상화
    - 제네릭 프로그래밍으로 전환이 되면 설계가 더 까다로워 진다.

    ● Where T : class
    
    - C#은 사용하는데 있어 조금 괜찮은 편 → 기본적으로 모든 데이터 타입에 동작하도록 설계가 되어야 한다.

    - 제네릭 클래스 또는 함수에 어떤 데이터 타입이 지정되어도 내부 로직에 변화가 발생하면 안된다.

    - 특정 데이터 타입에 동작하도록 데이터 타입을 제한하는 것이 가능하다.

    - T는 클래스만 받겠다는 제한 (참조형)
     ㄴ 결국 우리가 만든 함수는 Null 체크가 핵심 → int / float 들어오면 피곤해진다.

    - where T : class로 null이 될 수 있는 타입만 허용하겠다. → 실수 선행 방지

    ㆍ제네릭의 데이터 탕비 제한

    - Class CSomeClass<T> where T : class
     ㄴ 타입을 참조형식으로 제한

    - Class CSomeClass<T> where T : struct
     ㄴ 타입을 값 형식으로 제한

    - Class CSomeClass<T> where T : SomeClass
     ㄴ 클래스 씰 != 타입을 SomeClass를 직 / 간접적으로 상속하는 형식으로 제한

    - Class CSomeClass<T> where T : SomeInterface
     ㄴ 타입을 SomeInterface를 직 / 간접적으로 따르는 형식으로만 제한

    - Class CSomeClass<T, U> where T : U
     ㄴ 타입을 U로 직 / 간접적으로 상속하는 형식으로 제한
     */



    // 참조 체크
    public static void Ref(string label, UnityEngine.Object obj)
    {
        // 유니티에서 당연히 연결유무를 가장 빠르게 확인할 수 있는 로그
        //  ㄴ null 경고
        //  ㄴ 아니면 이름 출력

        // 유니티의 null은 참 모호한 존재

        if (obj == null)
        {
            Warn($"{label} : <null> (인스펙터 연결 필요)");
            return;
        }

        Log($"{label} : {obj.name}");
        
    }


    // Vector3

    public static void V3(string label, Vector3 v, int digits = 2)
    {
        // 반올림을 통해 로그를 읽기 쉽게 만든다.
        
        float x = (float)System.Math.Round(v.x, digits);
        float y = (float)System.Math.Round(v.y, digits);
        float z = (float)System.Math.Round(v.z, digits);

        // Math.Round → 함수
        // Mathf → Function
        Emit(ELogKind.Log, $"{label} : {x}, {y}, {z}");
    }


    public static void KV(string key, object value)
    {
        // KV = key = value 형태로 값을 찍는 표준 포맷 헬퍼
        // 우리가 디버깅할 때 → 가장 자주 찍는 형태 → 여기서 포맷 통일하겠다.

        /*
        Ex :

        CPrint.Group("Spawn Check", () =>
        {   
            CPrint.KV("PlayerPos", transform.position);
            CPrint.KV("HP",hp);
         });
         
         */

        Log($"{key} = {value}");
    }

    // 로그 규모가 커진다 → 섹션을 만든다. (로그 덩어리)
    public static void Group(string title, Action body, char lineCh = '=', int LineCount = 20)
    {
        if (!Enable)
        {
            return;
        }
        /*
        ▶ 델리게이트 → 간단 버젼

        - 이 또한 고급 문법..
        - 델리게이트는 함수를 변수철머 다룰 수 있게 해주는 타입
         ㄴ 특정 함수를 대신 호출해주는 대리자

        - 프로그래밍에서 델리게이트는 콜백 함수를 의미한다.
         ㄴ 델리게이트를 이용하면 특정 이벤트가 발생하는 시점에 해당 이벤트를 처리하는 것이 가능하다.

        - 대리자는 자기가 가르키고 있는 함수를 호출하는 역할을 한다.
         ㄴ 함수에 대한 참조를 들고 있다.

        [요약]
        1. 실행을 위임한다.
        2. 호출 주체와 실행 주체가 같지 않다.
        3. 콜백의 시작점


        ▶ Action

        - Action은 델리게이트의 미리 만들어진 형태 (표준)
         ㄴ 3총사 : Action<T> / Function<T, TResult> / Predicate<T>

        - 델리게이트는 함수를 담을 수 있는 타입

        - Action → 매개 변수 없고 / 반환값 없는 형태 → C# 기본 제공
         ㄴ 실행할 코드 덩어리를 → 변수처럼 전달한다.

        Ex :
        CPrint.Group("프리셋", () =>
        {
            CPrint.Log("색상 교체");
            CPrint.Log("재질 교체");
            CPrint.Log("라이트 교체");
        });

        - Action은 코드를 데이터처럼 넘긴다 라는 느낌 → Group은 로그 꾸러미를 함수로 받는 것

        */
        // 1. 그룹 제목 출력
        Title(title, lineCh);
        // 2. 그룹 내부 → 한 단계 들여쓰기
        IndentPush();
        // 실행할 코드 블록을 호출(Action)
        // ?.Invoke() : body가 null이면 실행하지 않겠다. → 예외 방지
        body?.Invoke();
        // 다시 복구 (들여쓰기)
        IndentPop();
        // 구분선 → 스타일 튜닝
        Line(lineCh, LineCount);
    }


    public static void Once(string key, string msg)
    {
        if (!Enable)
        {
            return;
        }

        // 이미 키가 있는 경우 → 재출력 금지
        if (_onceSet.Contains(key))
        {
            return;
        }

        _onceSet.Add(key);

        Warn($"Once {msg}");

        /*
        Ex :
        CPrint.Once("NoRB", "RigidBody가 없어 물리 이동이 안됨 (혹은 동작하지 않음)");
        */

    }

    public static void OnceClear()
    {
        // 등록된 키 전부 비운다.
        //   ㄴ 보통 씬 재시작 → 테스트 반복 환경에서 사용할 수 있음

        _onceSet.Clear();
    }


    // 에디터 / 개발 빌드에서만 남기고 싶은 함수  모음
    //  ㄴ 규모가 적으면 속성으로 처리를 하고 → 함수가 많아지면 → 선택적 컴파일로 처리하면 된다. 
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Ray(Vector3 origin, Vector3 direction, Color color, float duration = 0f)
    {
        if (!Enable)
        {
            return;
        }

        Debug.DrawRay(origin, direction, color, duration);




    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Line3D(Vector3 a, Vector3 b, Color color, float duration = 0f)
    {
        if (!Enable)
        {
            return;
        }

        Debug.DrawLine(a, b, color, duration);




    }

}
