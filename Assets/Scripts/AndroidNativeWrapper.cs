using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class AndroidNativeWrapper : MonoBehaviour
{
#if UNITY_ANDROID
    [DllImport("AndroidNative")]
    public static extern int LEDControl(int number);

    public int NativeLEDControl(int number)
    {
        return LEDControl(number);
    }

    [DllImport("AndroidNative")]
    public static extern int PiezoControl(int number);

    public int NativePiezoControl(int number)
    {
        return PiezoControl(number);
    }

    [DllImport("AndroidNative")]
    public static extern void PiezoControlSuccess();

    public void NativePiezoControlSuccess()
    {
        PiezoControlSuccess();
    }

    [DllImport("AndroidNative")]
    public static extern void PiezoControlFail();

    public void NativePiezoControlFail()
    {
        PiezoControlFail();
    }

    [DllImport("AndroidNative")]
    public static extern void SegmentControl(int number);

    public void NativeSegmentControl(int number)
    {
        SegmentControl(number);
    }

    [DllImport("AndroidNative")]
    public static extern void SegmentIOControl(int number);

    public void NativeSegmentIOControl(int number)
    {
        SegmentIOControl(number);
    }

    public void CustomSegmentControlTime(int number)
    {
        // 1초 간격으로 0~9까지 순차적으로 표시
        for (int i = 0; i < number; i++)
        {
            SegmentIOControl(1);
            for (int j = 0; j< 14; j++)
            {
                SegmentControl(i);
            }
            System.Threading.Thread.Sleep(1000);

        }
    }

    [DllImport("AndroidNative")]
    public static extern void TextLCDOut(string str1, string str2);

    public void NativeTextLCDControl(string str1, string str2)
    {
        TextLCDOut(str1, str2);
    }

    [DllImport("AndroidNative")]
    public static extern void IOCtlWriteByte(string data);

    public void NativeIOCtlWriteByte(string data)
    {
        IOCtlWriteByte(data);
    }

    [DllImport("AndroidNative")]
    public static extern void IOCtlPos(int pos);

    public void NativeIOCtlPos(int pos)
    {
        IOCtlPos(pos);
    }

    [DllImport("AndroidNative")]
    public static extern void IOCtlClear();

    public void NativeIOCtlClear()
    {
        IOCtlClear();
    }

    [DllImport("AndroidNative")]
    public static extern void IOCtlReturnHome();

    public void NativeIOCtlReturnHome()
    {
        IOCtlReturnHome();
    }

    [DllImport("AndroidNative")]
    public static extern void IOCtlDisplay(bool display);

    public void NativeIOCtlDisplay(bool display)
    {
        IOCtlDisplay(display);
    }

    [DllImport("AndroidNative")]
    public static extern void IOCtlCursor(bool cursor);

    public void NativeIOCtlCursor(bool cursor)
    {
        IOCtlCursor(cursor);
    }

    [DllImport("AndroidNative")]
    public static extern void IOCtlBlink(bool blink);

    public void NativeIOCtlBlink(bool blink)
    {
        IOCtlBlink(blink);
    }

    [DllImport("AndroidNative")]
    public static extern void FullColorLEDControl(int number, int value1, int value2, int value3);

    public void NativeFullColorLEDControl(int number, int value1, int value2, int value3)
    {
        FullColorLEDControl(number, value1, value2, value3);
    }

    public void CustomFullColorLEDSuccess()
    {
        // 초록불 0.2초 간격으로 1번 깜빡이고 끄기
        FullColorLEDControl(5, 0, 255, 0);
        System.Threading.Thread.Sleep(200);
        FullColorLEDControl(5, 0, 255, 0);
        System.Threading.Thread.Sleep(200);
        FullColorLEDControl(5, 0, 0, 0);
    }

    public void CustomFullColorLEDFail()
    {
        // 빨간불 0.2초 간격으로 1번 깜빡이고 끄기
        FullColorLEDControl(5, 255, 0, 0);
        System.Threading.Thread.Sleep(200);
        FullColorLEDControl(5, 255, 0, 0);
        System.Threading.Thread.Sleep(200);
        FullColorLEDControl(5, 0, 0, 0);
    }

    public void CustomFullColorRandom() {
        // 랜덤 색상으로 표현
        while (true) {
            FullColorLEDControl(5, UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255));
            System.Threading.Thread.Sleep(200);
        }
    }

    public void CustomFullColorLEDOff() {
        // LED 끄기
        FullColorLEDControl(5, 0, 0, 0);
    }

    [DllImport("AndroidNative")]
    public static extern void DotMatrixControl(string str);

    public void NativeDotMatrixControl(string str)
    {
        DotMatrixControl(str);
    }

#endif
}
