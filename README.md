# My First Net CORE app
Authentication: https://github.com/aspnet/AspNetCore/issues/2193



# Ubuntu
1. Setup nginx
    - https://www.digitalocean.com/community/tutorials/how-to-install-nginx-on-ubuntu-16-04
1. Prepare for dotnet
    - https://docs.microsoft.com/en-us/dotnet/core/linux-prerequisites?tabs=netcore2x
1. Install dotnet
    - https://dev.to/carlos487/installing-dotnet-core-in-ubuntu-1804-7lp

1. Setup dotnet hosting
    - https://odan.github.io/2018/07/17/aspnet-core-2-ubuntu-setup.html

1. Publish to local directory:
    - https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-2.2
1. SCP that crap
```
    - scp -i ~/.ssh/privateKey.pem -rp SourceDirectory ubuntu@ec2-18-217-145-143.us-east-2.compute.amazonaws.com:/home/ubuntu

    - scp -i {path to key.pem} -rp {source directory} USER@url.com:{/remote/directory/target}
```
1. Start NGINX
    - https://mediatemple.net/community/products/developer/204405534/install-nginx-on-ubuntu

1. Configure a new site for NGINX
    - https://waqarafridi.wordpress.com/2017/12/20/nginx-web-server-and-first-site/




