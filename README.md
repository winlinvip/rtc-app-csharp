# rtc-app-csharp

C# AppServer for RTC.

You could also write AppServer by following languages:

* Golang: https://github.com/winlinvip/rtc-app-golang
* Java: https://github.com/winlinvip/rtc-app-java
* Python: https://github.com/winlinvip/rtc-app-python
* C#: https://github.com/winlinvip/rtc-app-csharp
* Nodejs: https://github.com/winlinvip/rtc-app-nodejs
* PHP: https://github.com/winlinvip/rtc-app-php

For RTC deverloper:

* RTC [workflow](https://help.aliyun.com/document_detail/74889.html).
* RTC [token generation](https://help.aliyun.com/document_detail/74890.html).

Use OpenAPI to create channel:

* Golang: https://help.aliyun.com/document_detail/74890.html#channel-golang
* Java: https://help.aliyun.com/document_detail/74890.html#channel-java
* Python: https://help.aliyun.com/document_detail/74890.html#channel-python
* C#: https://help.aliyun.com/document_detail/74890.html#channel-csharp
* Nodejs: https://help.aliyun.com/document_detail/74890.html#channel-nodejs
* PHP: https://help.aliyun.com/document_detail/74890.html#channel-php

Token generation algorithm:

* Golang: https://help.aliyun.com/document_detail/74890.html#token-golang
* Java: https://help.aliyun.com/document_detail/74890.html#token-java
* Python: https://help.aliyun.com/document_detail/74890.html#token-python
* C#: https://help.aliyun.com/document_detail/74890.html#token-csharp
* Nodejs: https://help.aliyun.com/document_detail/74890.html#token-nodejs
* PHP: https://help.aliyun.com/document_detail/74890.html#token-php

## Usage

1. Generate AK from [here](https://usercenter.console.aliyun.com/#/manage/ak):

```
AccessKeyID: OGAEkdiL62AkwSgs
AccessKeySecret: 4JaIs4SG4dLwPsQSwGAHzeOQKxO6iw
```

2. Create APP from [here](https://rtc.console.aliyun.com/#/manage):

```
AppID: iwo5l81k
```

3. Clone SDK and add `aliyun-net-openapi-core` and `aliyun-net-openapi-rtc` to project:

```
git clone https://github.com/winlinvip/rtc-app-csharp.git &&
cd rtc-app-csharp &&
git clone https://github.com/aliyun/aliyun-openapi-net-sdk.git
```

4. Run project with args(replace access key and appid with yours):

```
--listen=8080 --access-key-id=OGAEkdiL62AkwSgs --access-key-secret=4JaIs4SG4dLwPsQSwGAHzeOQKxO6iw --appid=iwo5l81k --gslb=https://rgslb.rtc.aliyuncs.com
```

5. Verify  your AppServer by [here](http://ossrs.net/talks/ng_index.html#/rtc-check?schema=http&host=127.0.0.1&port=8080&path=/app/v1/login&room=1237&user=jzufp&password=12345678) or [verify token](http://ossrs.net/talks/ng_index.html#/token-check).

![AppServer Success](https://github.com/winlinvip/rtc-app-golang/raw/master/images/app-ok.png)

![AppServer Failed](https://github.com/winlinvip/rtc-app-golang/raw/master/images/app-failed.png)

![AppServer Error Recovered](https://github.com/winlinvip/rtc-app-golang/raw/master/images/app-recovered.png)

> Remark: You can setup client native SDK by `http://30.2.228.19:8080/app/v1`.

> Remark: Please use your AppServer IP instead by `ifconfig eth0`.

## History

* [391badf](https://github.com/winlinvip/rtc-app-csharp/commit/391badfba9e8059c90476c9fab20d8ab41b90e15), Use HTTP, x3 times faster than HTTPS.
* [8f15c74](https://github.com/winlinvip/rtc-app-csharp/commit/8f15c74804437f5e77f335bccdf62147b396ef2a), Log the request id and cost in ms.
* [0dc5727](https://github.com/winlinvip/rtc-app-csharp/commit/0dc572712c55fd0fcdfc9762d0cf752995488da2), [55ea589](https://github.com/winlinvip/rtc-app-csharp/commit/55ea5895819a87036d19482bf522185a641e7e52), Support recover for some OpenAPI error.
* [1b7aac5](https://github.com/winlinvip/rtc-app-csharp/commit/1b7aac5a065f52f663e1b4474eeee6b099124dc8), Set endpoint to get correct error.
* Support create channel and sign user token.
