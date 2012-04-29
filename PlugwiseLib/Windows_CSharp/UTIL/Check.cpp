//------------------ BEGIN LICENSE BLOCK ------------------
//
// Project : Green Code Lab Plugwyse Library
// Description :
// Author: Green Code Lab
// Website: http://greencodelab.fr
// Version: 1.0
// Supports: Windows
//
// Original project : http://plugwiselib.codeplex.com/
// Copyright (c) 2012 Green Code Lab
// Licensed under the GPL license.
// See http://www.gnu.org/licenses/gpl.html
//
//------------------- END LICENSE BLOCK -------------------

// Check.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <math.h>
#define _WIN32_DCOM
#include <iostream>
using namespace std;
#include <comdef.h>
#include <Wbemidl.h>

# pragma comment(lib, "wbemuuid.lib")

int GetValues(IWbemServices *pSvc,ULONG *ulVal ,int n)
{
	int nError = 0;
	 // Step 6: --------------------------------------------------
    // Use the IWbemServices pointer to make requests of WMI ----

    // For example, get the name of the operating system
    IEnumWbemClassObject* pEnumerator = NULL;
    HRESULT hres = pSvc->ExecQuery(
        bstr_t("WQL"), 
        bstr_t("SELECT * FROM Win32_PerfRawData_PerfOS_Processor"),
        WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY, 
        NULL,
        &pEnumerator);
    
    if (FAILED(hres))
    {
        cout << "Query for operating system name failed."
            << " Error code = 0x" 
            << hex << hres << endl;

        return 1;               // Fxn has failed.
    }

    // Step 7: -------------------------------------------------
    // Get the data from the query in step 6 -------------------
 
    IWbemClassObject *pclsObj;
    ULONG uReturn = 0;
	int nCtr = 0;
  
    while (nCtr<n)
    {
        HRESULT hr = pEnumerator->Next(WBEM_INFINITE, 1, 
            &pclsObj, &uReturn);

        if(0 == uReturn)
        {
            break;
        }

        VARIANT vtProp;
        VariantInit(&vtProp);

		hr = pclsObj->Get(L"PercentProcessorTime", 0, &vtProp, 0, 0);
		ulVal[nCtr] = _wtol(vtProp.bstrVal);
        VariantClear(&vtProp);
		hr = pclsObj->Get(L"TimeStamp_Sys100NS", 0, &vtProp, 0, 0);
		ulVal[nCtr+1] = _wtol(vtProp.bstrVal);
		VariantClear(&vtProp);
		nCtr+=2;
    }
	pclsObj->Release();
	pEnumerator->Release();
	return nError;
}


int main(int argc, char **argv)
{
    HRESULT hres;
    // Step 1: --------------------------------------------------
    // Initialize COM. ------------------------------------------
	int nproc=1;
    hres =  CoInitializeEx(0, COINIT_MULTITHREADED); 
    if (FAILED(hres))
    {
        cout << "Failed to initialize COM library. Error code = 0x" 
            << hex << hres << endl;
        return 1;                  // Program has failed.
    }

	cout << "Please enter the # of processors : ";

	cin>>nproc;
	ULONG *ulinitVal = new ULONG[(nproc+1)*2];//+1 for the total
	int i=0;
	ULONG *ulVal = new ULONG[(nproc+1)*2];//+1 for the total
    // Step 2: --------------------------------------------------
    // Set general COM security levels --------------------------
    // Note: If you are using Windows 2000, you need to specify -
    // the default authentication credentials for a user by using
    // a SOLE_AUTHENTICATION_LIST structure in the pAuthList ----
    // parameter of CoInitializeSecurity ------------------------

    hres =  CoInitializeSecurity(
        NULL, 
        -1,                          // COM authentication
        NULL,                        // Authentication services
        NULL,                        // Reserved
        RPC_C_AUTHN_LEVEL_DEFAULT,   // Default authentication 
        RPC_C_IMP_LEVEL_IMPERSONATE, // Default Impersonation  
        NULL,                        // Authentication info
        EOAC_NONE,                   // Additional capabilities 
        NULL                         // Reserved
        );

                      
    if (FAILED(hres))
    {
        cout << "Failed to initialize security. Error code = 0x" 
            << hex << hres << endl;
        CoUninitialize();
        return 1;                    // Program has failed.
    }
    
    // Step 3: ---------------------------------------------------
    // Obtain the initial locator to WMI -------------------------

    IWbemLocator *pLoc = NULL;

    hres = CoCreateInstance(
        CLSID_WbemLocator,             
        0, 
        CLSCTX_INPROC_SERVER, 
        IID_IWbemLocator, (LPVOID *) &pLoc);
 
    if (FAILED(hres))
    {
        cout << "Failed to create IWbemLocator object."
            << " Err code = 0x"
            << hex << hres << endl;
        CoUninitialize();
        return 1;                 // Program has failed.
    }

    // Step 4: -----------------------------------------------------
    // Connect to WMI through the IWbemLocator::ConnectServer method

    IWbemServices *pSvc = NULL;
	
    // Connect to the root\cimv2 namespace with
    // the current user and obtain pointer pSvc
    // to make IWbemServices calls.
    hres = pLoc->ConnectServer(
         _bstr_t(L"ROOT\\CIMV2"), // Object path of WMI namespace
         NULL,                    // User name. NULL = current user
         NULL,                    // User password. NULL = current
         0,                       // Locale. NULL indicates current
         NULL,                    // Security flags.
         0,                       // Authority (e.g. Kerberos)
         0,                       // Context object 
         &pSvc                    // pointer to IWbemServices proxy
         );
    
    if (FAILED(hres))
    {
        cout << "Could not connect. Error code = 0x" 
             << hex << hres << endl;
        pLoc->Release();     
        CoUninitialize();
        return 1;                // Program has failed.
    }

    cout << "Connected to ROOT\\CIMV2 WMI namespace" << endl;


    // Step 5: --------------------------------------------------
    // Set security levels on the proxy -------------------------

    hres = CoSetProxyBlanket(
       pSvc,                        // Indicates the proxy to set
       RPC_C_AUTHN_WINNT,           // RPC_C_AUTHN_xxx
       RPC_C_AUTHZ_NONE,            // RPC_C_AUTHZ_xxx
       NULL,                        // Server principal name 
       RPC_C_AUTHN_LEVEL_CALL,      // RPC_C_AUTHN_LEVEL_xxx 
       RPC_C_IMP_LEVEL_IMPERSONATE, // RPC_C_IMP_LEVEL_xxx
       NULL,                        // client identity
       EOAC_NONE                    // proxy capabilities 
    );

    if (FAILED(hres))
    {
        cout << "Could not set proxy blanket. Error code = 0x" 
            << hex << hres << endl;
        pSvc->Release();
        pLoc->Release();     
        CoUninitialize();
        return 1;               // Program has failed.
    }
	
	char szVal[64];
	
	while(i++<1000)
	{
		//Get the initial Values
		if(GetValues(pSvc,ulinitVal,(nproc+1)*2))
		{
			delete ulinitVal;
			goto Cleanup;
		}

		Sleep(2000);

		//Get the initial Values
		GetValues(pSvc,ulVal,(nproc+1)*2);

		if((ulVal[1]- ulinitVal[1])>0)
		{
			sprintf(szVal,"%0.2f",fabs(100.0- ((float)(ulVal[0] - ulinitVal[0]) / (ulVal[1]- ulinitVal[1])) * 100));
			cout<< "PercentProcessorTime CPU 0: \t\t" <<szVal<<endl;
		}
		else
		{
			cout<<"Divide by zero for Processor 0"<<endl;
		}

		if(nproc>1)
		{
			if((ulVal[3]- ulinitVal[3])>0)
			{
				sprintf(szVal,"%0.2f",fabs(100.0- ((float)(ulVal[2] - ulinitVal[2]) / (ulVal[3]- ulinitVal[3])) * 100));
				cout<< "PercentProcessorTime CPU 1:\t\t" <<szVal<<endl;
			}
			else
			{
				cout<<"Divide by zero for Processor 1"<<endl;
			}
		
			if((ulVal[5]- ulinitVal[5])>0)
			{
				sprintf(szVal,"%0.2f",fabs(100.0- ((float)(ulVal[4] - ulinitVal[4]) / (ulVal[5]- ulinitVal[5])) * 100));	
				cout<< "PercentProcessorTime  CPU Total:\t" <<szVal<<endl;
			}
			else
			{
				cout<<"Divide by zero for total"<<endl;
			}
		}
		else
		{
			if((ulVal[3]- ulinitVal[3])>0)
			{
				sprintf(szVal,"%0.2f",fabs(100.0-((float)(ulVal[2] - ulinitVal[2]) / (ulVal[3]- ulinitVal[3])) * 100));
				cout<< "PercentProcessorTime  CPU Total:\t" << szVal<<endl;
			}
			else
			{
				cout<<"Divide by zero for total"<<endl;
			}
		}


	}
	delete ulinitVal;
	delete ulVal;

Cleanup:
   
    // Cleanup
    // ========
    
    pSvc->Release();
    pLoc->Release();
    CoUninitialize();

	getchar();
    return 0;   // Program successfully completed.
	
}