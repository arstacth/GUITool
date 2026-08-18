# ---------------------------------------------------------------
#
# Copyright 2005 by Rhaon Ent.
#
# GUIdemo_startup.py
#
# 2005.2. Pyo, Taesu.
# 
# ---------------------------------------------------------------

import random
import guiutil

random.seed()

guiutil.setParentCtrlNone()

# exit button
guiutil.createCtrl("Button")
guiutil.setCommandID("EXIT")
guiutil.setText("종료")
guiutil.setPos(0, 0, 60, 30)
guiutil.setAlign(gui.HALIGN.RIGHT, gui.VALIGN.BOTTOM)


# message wnd
guiutil.createCtrl("MessageWnd")
guiutil.setPos(10, 100, 200, 200)
guiutil.setName("MessageWnd")

# addmessage button
guiutil.createCtrl("Button")
guiutil.setCommandID("AddMessage")
guiutil.setText("AddMessage")
guiutil.setPos(10, 320, 100, 30)



# listbox
guiutil.createCtrl("ListBox")
guiutil.setName("ListBox")
guiutil.setPos(250, 50, 400, 350)

# addtolist button
guiutil.createCtrl("Button")
guiutil.setCommandID("AddToList")
guiutil.setText("AddToList")
guiutil.setPos(250, 420, 100, 30)

# text file button
guiutil.createCtrl("Button")
guiutil.setCommandID("LoadText")
guiutil.setText("LoadText")
guiutil.setPos(370, 420, 100, 30)

guiutil.createCtrl("Button")
guiutil.setCommandID("ClearListBox")
guiutil.setText("ClearListBox")
guiutil.setPos(490, 420, 100, 30)


# edit
guiutil.createCtrl("EditCtrl")
guiutil.setName("EditCtrl")
guiutil.setPos(400, 480, 200, 30)
guiutil.setCommandID("EditCtrlReturnKey")

# radio button
guiutil.createCtrl("RadioButton")
guiutil.setText("라디오 Radio1")
guiutil.setPos(20, 450, 100, 30)

guiutil.createCtrl("RadioButton")
guiutil.setText("Radio2")
guiutil.setPos(150, 450, 100, 30)

guiutil.createCtrl("RadioButton")
guiutil.setText("Radio3")
guiutil.setPos(250, 450, 100, 30)

# check button
guiutil.createCtrl("CheckButton")
guiutil.setText("Check1")
guiutil.setPos(20, 500, 100, 30)

guiutil.createCtrl("CheckButton")
guiutil.setText("체크 Check2")
guiutil.setPos(20, 530, 100, 30)

# dialog

guiutil.createCtrl("Dialog")
guiutil.setPos(50, 150, 200, 200)

guiutil.setParentCtrl()

guiutil.createCtrl("EditCtrl")
guiutil.setPos(20,20, 150, 30)

guiutil.createCtrl("Button")
guiutil.setPos(50,150, 100, 30)
guiutil.setText("테스트")



