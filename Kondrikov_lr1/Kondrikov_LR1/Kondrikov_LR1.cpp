// Kondrikov_LR11.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "pch.h"
#include "framework.h"
#include "Kondrikov_LR1.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// The one and only application object

CWinApp theApp;

using namespace std;

struct threadParam
{
    int number = 0;
    vector<HANDLE>* handleVec;
};


// function to control coutput thread creation/deletion
DWORD __stdcall ThreadCtrl(LPVOID lpParameter)
{
    threadParam* threadInfo = (threadParam*)lpParameter;

    // printf("thread number %d has been generated\n", ThreadNum);
    string sc1 = "thread number ";
    string sc2 = " has been generated\n";
    string outc = sc1 + to_string(threadInfo->number) + sc2;
    cout << outc;
  
    WaitForSingleObject((*threadInfo->handleVec)[threadInfo->number], INFINITE);

    // printf("thread number %d has gone\n", ThreadNum);
    string st1 = "thread number ";
    string st2 = " has gone\n";
    string outt = st1 + to_string(threadInfo->number) + st2;
    cout << outt;

    return 0;
}


// main function to manage treads
void StartApp()
{
    HANDLE StopEventHandle = CreateEvent(NULL, FALSE, FALSE, "StopEvent");
    HANDLE StartEventHandle = CreateEvent(NULL, FALSE, FALSE, "StartEvent");
    HANDLE ConfirmEventHandle = CreateEvent(NULL, FALSE, FALSE, "ConfirmEvent");

    threadParam param;
    vector<HANDLE> runVector;
    param.handleVec = &runVector;
    param.number = 0;

    HANDLE SuperEventHandle[2] = { StartEventHandle, StopEventHandle };
    
    int i = 0;
    while(true)
    { 
        switch (WaitForMultipleObjects(2, SuperEventHandle, FALSE, INFINITE) - WAIT_OBJECT_0)
        {
            case 0:
            {
                param.number = i;
                runVector.push_back(CreateEvent(NULL, FALSE, FALSE, NULL));
                CreateThread(NULL, 0, ThreadCtrl, (LPVOID)&param, 0, NULL);
                SetEvent(ConfirmEventHandle);
                ++i;
                break;
            }
            case 1:
            {
                if (param.handleVec->empty())
                {
                    SetEvent(ConfirmEventHandle);
                    return;
                }
                else
                {
                    param.number = --i;
                    SetEvent(runVector[param.number]);
                    CloseHandle(runVector[param.number]);
                    SetEvent(ConfirmEventHandle);
                    runVector.pop_back();
                }
                break;
            }
            default:
            {
                break;
            }
        }
    }
}



int main()
{
 
// vars handlers 
    vector<HANDLE> ThreadEventHandler;

    int nRetCode = 0;

    HMODULE hModule = ::GetModuleHandle(nullptr);

    if (hModule != nullptr)
    {
        // initialize MFC and print and error on failure
        if (!AfxWinInit(hModule, nullptr, ::GetCommandLine(), 0))
        {
            // TODO: code your application's behavior here.
            wprintf(L"Fatal Error: MFC initialization failed\n");
            nRetCode = 1;
        }
        else
        {
            StartApp();
        }
    }
    else
    {
        // TODO: change error code to suit your needs
        wprintf(L"Fatal Error: GetModuleHandle failed\n");
        nRetCode = 1;
    }

    return nRetCode;
}
