# rtc-app-csharp

C# AppServer for RTC.

You could also write AppServer by following languages:

* Python: https://github.com/winlinvip/rtc-app-python
* Java: https://github.com/winlinvip/rtc-app-java
* Golang: https://github.com/winlinvip/rtc-app-golang
* PHP: https://github.com/winlinvip/rtc-app-php
* C#: https://github.com/winlinvip/rtc-app-csharp
* Nodejs: https://github.com/winlinvip/rtc-app-nodejs

For RTC deverloper:

* RTC [workflow](https://help.aliyun.com/document_detail/74889.html).
* RTC [token generation](https://help.aliyun.com/document_detail/74890.html).

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

5. Verify AppServer by [here](http://localhost:8080/app/v1/login?room=5678&user=nvivy&passwd=12345678).

> Remark: You can setup client native SDK by `http://30.2.228.19:8080/app/v1`.

> Remark: Please use your AppServer IP instead by `ifconfig eth0`.
