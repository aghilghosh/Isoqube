import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Utilities } from '../../helpers/utilities';
import { DefaultService } from '../../services/default.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-default',
  templateUrl: './default.component.html',
  styleUrls: ['./default.component.css']
})

export class DefaultComponent implements OnInit {

  emailId: string = 'admin@isoqube.com';
  public _environment = environment;
  constructor(private defaultService: DefaultService, private router: Router) {}

  ngOnInit() {   
  }

  proceedToOrganizations() {
    
    var signedToken = Utilities.signToken(btoa(this.emailId), Utilities.SIGN_TOKEN_KEY);
    localStorage.setItem(Utilities.LOCAL_TOKEN_ID, signedToken);

    this.router.navigate(['session/orchestrations']);
  }

  validateEmail(): boolean {
    return Utilities.validateEmail(this.emailId);
  }

  title = environment.APP_ECOSYSTEM;
}
