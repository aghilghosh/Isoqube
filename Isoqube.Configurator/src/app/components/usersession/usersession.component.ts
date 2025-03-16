import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Utilities } from '../../helpers/utilities';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-usersession',
  templateUrl: './usersession.component.html',
  styleUrls: ['./usersession.component.css']
})
export class UserSessionComponent implements OnInit {

  public _environment = environment;
  constructor(private router: Router) { }

  ngOnInit() {
  }

  terminateUserSession() {
    localStorage.removeItem(Utilities.LOCAL_TOKEN_ID);
    this.router.navigate(['default']);
  }
}
