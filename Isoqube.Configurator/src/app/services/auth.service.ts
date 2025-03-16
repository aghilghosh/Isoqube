import { Injectable } from '@angular/core';
import { CanActivate, CanActivateChild, CanDeactivate, CanLoad, Router } from '@angular/router';
import { Utilities } from '../helpers/utilities';

@Injectable({
  providedIn: 'root'
})

export class AuthGuard implements CanActivate, CanActivateChild, CanLoad {
  constructor(private router: Router) { }

  canActivate(): boolean {
    return this.checkAuth();
  }

  canActivateChild(): boolean {
    return this.checkAuth();
  }

  canLoad(): boolean {
    return this.checkAuth();
  }

  private checkAuth(): boolean {

    let isValid = false;
    let localEmailToken = localStorage.getItem(Utilities.LOCAL_TOKEN_ID);

    if (localEmailToken)
    {
      
      let email = Utilities.decodeJwt(localEmailToken);
      isValid = Utilities.validateEmail(email);
    }

    if (!isValid) {
      console.log('FAILED: Token authentication')
      this.router.navigate(['/default']);
      return isValid;
    }

    return isValid;
  }

}
