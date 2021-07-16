#import <Foundation/Foundation.h>

char* MycStringCopy(const char* string)
{
    if (string == NULL)
        return NULL;
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

char* _getNativeLocale() {
    NSString *language = [[NSLocale currentLocale] objectForKey:NSLocaleCountryCode];
    return MycStringCopy([language UTF8String]);
}