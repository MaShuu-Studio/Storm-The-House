using System;
using System.Collections.Generic;
using System.Text;

namespace Excel_To_Json
{
    class JsonFormat
    {
        // {0} 콘텐츠 목록 (contentsFormat)
        public static string jsonFormat =
@"
{{
    ""list"": 
    [
        {0}
    ]
}}
";

        // {0} 기본 변수(valueFormats)
        // {1} 데이터 타입(enum)
        // {2} 데이터 내용(valueFormat)
        public static string itemContentsFormat =
@"
{{
    {0}
    ""data"":
    {{
        ""keys"": 
        [
            {1}
        ],
        ""values"": 
        [
            {2}
        ]
    }}
}}";
        // {0} 타입 이름
        // {1} 타입 값
        public static string valueFormat =
@"""{0}"": {1}";

        // {0} 데이터 내용(valueFormat)
        public static string enemyContentsFormat =
@"
{{
    ""enemies"":
    [
        {0}
    ]
}}";
        // {0} 데이터 내용(vauleFormat)
        public static string basicContentsFormat =
@"
{{
    {0}
}}";
    }
}
