import { keepSessionAlive, heartbeatInterval } from './learningMenu/keepSessionAlive';

// send out a heartbeat, to keep this session alive, once a minute
setInterval(keepSessionAlive, heartbeatInterval);
