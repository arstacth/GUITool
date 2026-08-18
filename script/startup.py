# ---------------------------------------------------------------
#
# Copyright 2004 by Rhaon Ent.
#
# startup.py
#
# 2005.12. Pyo, Taesu.
# 
# ---------------------------------------------------------------

import random

random.seed()


# -------------------------------------------------
#
# GUI 폰트 설정.
#
# addTextFont(폰트번호, 폰트이름, Bold?, 폰트크기px, GdiPlus?, Antialias?)

# 한국어 폰트.
# "Normal"
addTextFont(0, "굴림", True, 16, False)

# "Big"
addTextFont(1, "굴림", True, 26, True)

# "Small"
addTextFont(2, "굴림", False, 12, False)

# "Mini"
addTextFont(3, "굴림", False, 11, False, False)

# "Small_B"
addTextFont(4, "굴림", True, 12, False)

# "HY_GE_12"
addTextFont(5, "H2GTRE.ttf", False, 12, True)

# "HY_GE_14"
addTextFont(6, "H2GTRE.ttf", False, 14, True)

# "HY_GE_16"
addTextFont(7, "H2GTRE.ttf", False, 16, True)

# "HY_GE_18"
addTextFont(8, "H2GTRE.ttf", False, 18, True)

# "HY_GE_13"
addTextFont(9, "H2GTRE.ttf", False, 13, True)

# "Mini_B"
addTextFont(10, "굴림", True, 11, False, False)

# "HY_GE_11"
addTextFont(11, "H2GTRE.ttf", False, 11, True)

# "HY_GE_24"
addTextFont(12, "H2GTRE.ttf", False, 24, True)

# "HY_GE_28"
addTextFont(13, "H2GTRE.ttf", False, 28, True)

# "HY_GE_36"
addTextFont(14, "H2GTRE.ttf", False, 36, True)

# "UHBEE_16"
addTextFont(17, "UhBee namsoyoung.ttf", False, 16, True)

# "NANUMMB_14"
addTextFont(15, "NanumMyeongjoBold.ttf", False, 14, True)

# "NANUMMB_18"
addTextFont(16, "NanumMyeongjoBold.ttf", False, 18, True)

# "NANUMSB_14"
addTextFont(18, "NanumSquareB.ttf", False, 14, True)

# "NANUMSB_16"
addTextFont(19, "NanumSquareB.ttf", False, 16, True)

# "NANUMSB_18"
addTextFont(20, "NanumSquareB.ttf", False, 18, True)

# "NANUMSB_36"
addTextFont(21, "NanumSquareB.ttf", False, 36, True)

# "NANUMSEB_14"
addTextFont(22, "NanumSquareEB.ttf", False, 14, True)

# "NANUMSEB_16"
addTextFont(23, "NanumSquareEB.ttf", False, 16, True)

# "NANUMSEB_18"
addTextFont(24, "NanumSquareEB.ttf", False, 18, True)

# "NANUMSEB_36"
addTextFont(25, "NanumSquareEB.ttf", False, 36, True)

# "NANUMGT_14"
addTextFont(26, "NanumBarunGothic.ttf", False, 14, True)

# "NANUMGT_B_14"
addTextFont(27, "NanumBarunGothicBold.ttf", True, 14, True)

# -------------------------------------------------
#
# Level Tool 설정.

# 팜 그리드 Size
setProperty("FarmGridSize","100.f")


