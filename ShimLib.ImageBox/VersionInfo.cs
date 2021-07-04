using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    class VersionInfo {
        public const string History =
@"## Version Info
### V1.0.0.2 - 2021.06.12
1. BufferedGraphics 사용 안하고 DibSection 직접 생성하여 사용하여 속도 개선

### V1.0.0.1 - 2021.06.8
1. mame uismall.bdf 가변폭 폰트 추가
2. infoFont 속성 추가
3. Ascii_04x06_2 폰트 추가
4. void Redraw() -> new void Invalidate() 함수명 변경
5. ImageDrawing, ImageGraphics 클래스에 대해 ImageBox 의존성 제거
6. LineDrawAction Custom assign 기능 추가
7. 속도 개선 위해 HLine, VLIne 클리핑 처리

### V1.0.0.0 - 2021.05.28
1. todo.txt Solution Item폴더로 이동
2. VersionInfo.md 리소스 추가 및 설정창에 표시";
    }
}
