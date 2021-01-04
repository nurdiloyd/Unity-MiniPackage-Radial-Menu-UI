
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface ElephantController : NSObject
@end

#ifdef __cplusplus
extern "C" {
#endif

const char* IDFA();
void TestFunction(const char * string);
void ElephantPost(const char * url, const char * body, const char * gameID, const char * authToken, int tryCount);

const char * ElephantCopyString(const char * string)
{
   char * copy = (char*)malloc(strlen(string) + 1);
   strcpy(copy, string);
   return copy;
}

    
#ifdef __cplusplus
} // extern "C"
#endif


NS_ASSUME_NONNULL_END
