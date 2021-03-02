import { Injectable } from '@angular/core';
import { UserManager, User, UserManagerSettings } from 'oidc-client';
import { Constants } from '../constants';
import { Subject } from 'rxjs';


@Injectable({
  providedIn: 'root'
})

export class AuthService {

  private _loginChangedSubject = new Subject<boolean>();
  private _userManager: UserManager;
  private _user: User;
  public loginChanged = this._loginChangedSubject.asObservable();

  private checkUser = (user : User): boolean => {
    return !!user && !user.expired;
  }

  private get idpSettings() : UserManagerSettings {
    return {
      authority: Constants.idpAuthority,
      client_id: Constants.clientId,
      redirect_uri: `${Constants.clientRoot}/signin-callback`,
      scope: "openid profile companyApi",
      // response_type: "code",
      response_type: "code",
      post_logout_redirect_uri: `${Constants.clientRoot}/signout-callback`
    }
  }

  constructor() {
    this._userManager = new UserManager(this.idpSettings);
   }

   public login = () => {
    return this._userManager.signinRedirect();
  }

  public finishLogin = (): Promise<User> => {
    return this._userManager.signinRedirectCallback()
    .then(user => {
      this._user = user;
      this._loginChangedSubject.next(this.checkUser(user));
      return user;
    })
  }

  public logout = () => {
    this._userManager.signoutRedirect();
  }

  public finishLogout = () => {
    this._user = null;
    return this._userManager.signoutRedirectCallback();
  }


  public isAuthenticated = (): Promise<boolean> => {
    return this._userManager.getUser()
    .then(user => {
      if(this._user !== user){
        this._loginChangedSubject.next(this.checkUser(user));
      }
      this._user = user;
      return this.checkUser(user);
    })
  }

  public getAccessToken = (): Promise<string> => {
    return this._userManager.getUser()
      .then(user => {
         return !!user && !user.expired ? user.access_token : null;
    })
  }



}
