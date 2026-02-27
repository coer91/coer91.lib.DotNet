namespace coer91.NET
{ 
    public abstract class ResponseDTOBuilder<T> : ResponseAbstract<T>
    {
        #region OK
        /// <summary>
        /// HTTP Code 200
        /// </summary>
        public ResponseDTO<T> Ok(T data, string message = "Success")
            => Ok(data, [message]);

        /// <summary>
        /// HTTP Code 200
        /// </summary>
        public ResponseDTO<T> Ok(string message = "Success")
            => Ok([message]);

        /// <summary>
        /// HTTP Code 200
        /// </summary>
        public ResponseDTO<T> Ok(IEnumerable<string> messageList)
            => Ok(default, messageList); 

        /// <summary>
        /// HTTP Code 200
        /// </summary>
        public ResponseDTO<T> Ok(T data, IEnumerable<string> messageList)
        {
            Failure = false;
            HttpCode = 200;
            MessageList = [.. messageList];

            return new ResponseDTO<T>()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList,
                Data = data
            };
        }
        #endregion


        #region Created 
        /// <summary>
        /// HTTP Code 201
        /// </summary>
        public ResponseDTO<T> Created(T data, string message = "Created")
            => Created(data, [message]);

        /// <summary>
        /// HTTP Code 201
        /// </summary>
        public ResponseDTO<T> Created(string message = "Created")
            => Created([message]);

        /// <summary>
        /// HTTP Code 201
        /// </summary>
        public ResponseDTO<T> Created(IEnumerable<string> messageList)
            => Created(default, messageList);  

        /// <summary>
        /// HTTP Code 201
        /// </summary>
        public ResponseDTO<T> Created(T data, IEnumerable<string> messageList)
        {
            Failure = false;
            HttpCode = 201;
            MessageList = [.. messageList];

            return new ResponseDTO<T>()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList,
                Data = data
            };
        }
        #endregion


        #region NoContent
        /// <summary>
        /// HTTP Code 204
        /// </summary>
        public ResponseDTO<T> NoContent(string message = "No Content")
            => NoContent([message]);

        /// <summary>
        /// HTTP Code 204
        /// </summary>
        public ResponseDTO<T> NoContent(IEnumerable<string> messageList)
        {
            Failure = false;
            HttpCode = 204;
            MessageList = [.. messageList];

            return new ResponseDTO<T>()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region BadRequest
        /// <summary>
        /// HTTP Code 400
        /// </summary>
        public ResponseDTO<T> BadRequest(string message = "Bad Request")
            => BadRequest([message]);

        /// <summary>
        /// HTTP Code 400
        /// </summary>
        public ResponseDTO<T> BadRequest(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 400;
            MessageList = [.. messageList];

            return new ResponseDTO<T>()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region Unauthorize
        /// <summary>
        /// HTTP Code 401
        /// </summary>
        public ResponseDTO<T> Unauthorize(string message = "Unauthorize")
            => Unauthorize([message]);

        /// <summary>
        /// HTTP Code 401
        /// </summary>
        public ResponseDTO<T> Unauthorize(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 401;
            MessageList = [.. messageList];

            return new ResponseDTO<T>()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region Forbidden
        /// <summary>
        /// HTTP Code 403
        /// </summary>
        public ResponseDTO<T> Forbidden(string message = "Forbidden")
            => Forbidden([message]);

        /// <summary>
        /// HTTP Code 403
        /// </summary>
        public ResponseDTO<T> Forbidden(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 403;
            MessageList = [.. messageList];

            return new ResponseDTO<T>()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region NotFound
        /// <summary>
        /// HTTP Code 404
        /// </summary>
        public ResponseDTO<T> NotFound(string message = "Not Found")
            => NotFound([message]);

        /// <summary>
        /// HTTP Code 404
        /// </summary>
        public ResponseDTO<T> NotFound(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 404;
            MessageList = [.. messageList];

            return new ResponseDTO<T>()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region NotAcceptable
        /// <summary>
        /// HTTP Code 406
        /// </summary>
        public ResponseDTO<T> NotAcceptable(string message = "Not Acceptable")
            => NotAcceptable([message]);

        /// <summary>
        /// HTTP Code 406
        /// </summary>
        public ResponseDTO<T> NotAcceptable(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 406;
            MessageList = [.. messageList];

            return new ResponseDTO<T>()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region Conflict
        /// <summary>
        /// HTTP Code 409
        /// </summary>
        public ResponseDTO<T> Conflict(string message = "Conflict")
            => Conflict([message]);

        /// <summary>
        /// HTTP Code 409
        /// </summary>
        public ResponseDTO<T> Conflict(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 409;
            MessageList = [.. messageList];

            return new ResponseDTO<T>()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region PayloadTooLarge
        /// <summary>
        /// HTTP Code 413
        /// </summary>
        public ResponseDTO<T> PayloadTooLarge(string message = "Payload Too Large")
            => PayloadTooLarge([message]);

        /// <summary>
        /// HTTP Code 413
        /// </summary>
        public ResponseDTO<T> PayloadTooLarge(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 413;
            MessageList = [.. messageList];

            return new ResponseDTO<T>()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region Error
        /// <summary>
        /// Default: HTTP Code 500
        /// </summary>
        public ResponseDTO<T> Error(string message = "Error", int httpCode = 500)
            => Error([message], httpCode);

        /// <summary>
        /// Default: HTTP Code 500
        /// </summary>
        public ResponseDTO<T> Error(IEnumerable<string> messageList, int httpCode = 500)
        {
            Failure = true;
            MessageList = [.. messageList];
            HttpCode = httpCode;

            return new ResponseDTO<T>()
            {
                Failure = Failure,
                MessageList = MessageList,
                HttpCode = HttpCode
            };
        }
        #endregion


        #region Exception
        /// <summary>
        /// Default: HTTP Code 500
        /// </summary>
        public ResponseDTO<T> Exception(Exception ex, int httpCode = 500)
        {
            Failure = true;
            HttpCode = httpCode;
            MessageList = [ex.InnerException?.Message ?? ex.Message];

            return new ResponseDTO<T>()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion
    }
} 