#include <stdio.h>
#include <string.h>
#include <sys/types.h>
//#include <sys/socket.h>
//#include <netinet/in.h>
//#include <arpa/inet.h>
//#include <netdb.h>
#include <errno.h>
#include <Ws2tcpip.h>
#include <Winsock2.h>
//typedef struct in_addr {
//        u_long          s_addr;
//} In_Addr;

//int h_errno;

struct in_addr* ipaddr;
#ifdef __cplusplus
extern "C"
{
#endif

    int
        main(int argc, char* argv[])
    {

        char* ptr, ** pptr;
        //char str[46];
        struct hostent* hptr;
        WORD wVersionRequested;
        WSADATA wsaData;
        SYSTEM_INFO sSystemInfo;
        int err;

        wVersionRequested = MAKEWORD(1, 1);
        err = WSAStartup(wVersionRequested, &wsaData);

        GetSystemInfo(&sSystemInfo);
        printf("sSystenInfo = %d\n", sSystemInfo.dwProcessorType);

        while (--argc > 0)
        {
            ptr = *++argv;
            if ((hptr = getaddrinfo(ptr)) == NULL)
            {
                printf("gethostbyname error for host: %s: %d", ptr, h_errno);
                continue;
            }
            printf("Official hostname: %s\n", (char*)hptr->h_name);

            for (pptr = hptr->h_aliases; *pptr != NULL; pptr++)
                printf("\talias: %s\n", *pptr);

            switch (hptr->h_addrtype)
            {
            case AF_INET:
#ifdef AF_INET6
            case AF_INET6:
#endif
                pptr = hptr->h_addr_list;
                for (; *pptr != NULL; pptr++)
                {
                    ipaddr = (struct in_addr*)*pptr;
                    printf("\taddress: %s\n", inet_ntop(*ipaddr));
                }
                break;

            default:
                printf("unknown address type");
                break;
            }
        }
        WSACleanup();
        return 0;
    }

#ifdef __cplusplus
}
#endif
