#import "ElephantController.h"
#import <AdSupport/AdSupport.h>

@implementation ElephantController



void TestFunction(const char * a){
    if(a != nullptr)
        NSLog(@"From Unity -> %s", a);
}

const char* IDFA(){
    NSString *emptyUserIdfa = @"00000000-0000-0000-0000-000000000000";
    NSUUID *u = [[ASIdentifierManager sharedManager] advertisingIdentifier];
    const char *a = [[u UUIDString] cStringUsingEncoding:NSUTF8StringEncoding];
    if ([emptyUserIdfa isEqualToString:[NSString stringWithUTF8String:a]]) {
        return ElephantCopyString("");
    } else {
        return ElephantCopyString(a);
    }
}

void ElephantPost(const char * _url, const char * _body, const char * _gameID, const char * _authToken, int tryCount){


    
            const char * url1 = ElephantCopyString(_url);
            const char * body1 = ElephantCopyString(_body);
            const char * gameID1 = ElephantCopyString(_gameID);
            const char * authToken1 = ElephantCopyString(_authToken);
            
            NSString *urlSt = [NSString stringWithCString:url1 encoding:NSUTF8StringEncoding];
            NSString *body = [NSString stringWithCString:body1 encoding:NSUTF8StringEncoding];
            NSString *gameID = [NSString stringWithCString:gameID1 encoding:NSUTF8StringEncoding];
            NSString *authToken = [NSString stringWithCString:authToken1 encoding:NSUTF8StringEncoding];
            
            NSError *error;

            NSURL *url = [NSURL URLWithString:urlSt];
            NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:url
                                                                   cachePolicy:NSURLRequestUseProtocolCachePolicy
                                                               timeoutInterval:300.0];

            
            [request addValue:@"application/json; charset=utf-8" forHTTPHeaderField:@"Content-Type"];
            [request addValue:authToken forHTTPHeaderField:@"Authorization"];
            [request addValue:gameID forHTTPHeaderField:@"GameID"];

            [request setHTTPMethod:@"POST"];

            NSData *requestBodyData = [body dataUsingEncoding:NSUTF8StringEncoding];
            [request setHTTPBody:requestBodyData];


            NSURLSessionDataTask *postDataTask = [[NSURLSession sharedSession] dataTaskWithRequest:request completionHandler:^(NSData *data, NSURLResponse *response, NSError *error) {
                bool failed = false;
                if(error != nil){
                    failed = true;
                }
                else if ([response isKindOfClass:[NSHTTPURLResponse class]]){
                    NSHTTPURLResponse *r = (NSHTTPURLResponse*)response;
                    if(r.statusCode != 200){
                        failed = true;
                    }
                }
                
                if(failed){
                    NSDictionary *failedReq = @{ @"url": urlSt, @"data": body, @"tryCount": [NSNumber numberWithInt:tryCount] };
                            NSData *jsonData = [NSJSONSerialization dataWithJSONObject:failedReq options:NSJSONWritingPrettyPrinted error:nil];
                            NSString *jsonSt = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                            UnitySendMessage("Elephant", "FailedRequest", [jsonSt cStringUsingEncoding:NSUTF8StringEncoding]);
                }
                
            }];

            [postDataTask resume];
            
            
            
            free((void*)url1);
            free((void*)body1);
            free((void*)gameID1);
            free((void*)authToken1);
   
}


@end
